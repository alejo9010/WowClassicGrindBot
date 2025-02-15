﻿using Core.Database;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using Cyotek.Collections;
using Cyotek.Collections.Generic;

namespace Core
{
    public sealed class AddonReader : IAddonReader, IDisposable
    {
        private readonly ILogger logger;
        private readonly ISquareReader squareReader;
        private readonly IAddonDataProvider addonDataProvider;
        public bool Active { get; set; } = true;
        public PlayerReader PlayerReader { get; set; }
        public BagReader BagReader { get; set; }
        public EquipmentReader equipmentReader { get; set; }

        public ActionBarCostReader ActionBarCostReader { get; set; }
        public LevelTracker LevelTracker { get; set; }

        public event EventHandler? AddonDataChanged;
        public event EventHandler? ZoneChanged;

        private readonly AreaDB areaDb;
        public WorldMapAreaDB WorldMapAreaDb { get; set; }
        private readonly ItemDB itemDb;
        private readonly CreatureDB creatureDb;


        private int seq = 0;

        private long lastGlobalTime = 0;
        private DateTime lastGlobalTimeChange = DateTime.Now;
        private readonly CircularBuffer<double> UpdateLatencys;

        private DateTime lastFrontendUpdate = DateTime.Now;
        private readonly int FrontendUpdateIntervalMs = 250;

        public AddonReader(ILogger logger, DataConfig dataConfig, AreaDB areaDb, IAddonDataProvider addonDataProvider)
        {
            this.logger = logger;
            this.addonDataProvider = addonDataProvider;

            this.squareReader = new SquareReader(this);

            this.itemDb = new ItemDB(logger, dataConfig);
            this.creatureDb = new CreatureDB(logger, dataConfig);

            this.equipmentReader = new EquipmentReader(squareReader, 24, 25);
            this.BagReader = new BagReader(squareReader, itemDb, equipmentReader, 20, 21, 22, 23);
            
            this.ActionBarCostReader = new ActionBarCostReader(squareReader, 36);
            this.PlayerReader = new PlayerReader(squareReader, creatureDb);
            this.LevelTracker = new LevelTracker(PlayerReader);

            this.areaDb = areaDb;
            this.WorldMapAreaDb = new WorldMapAreaDB(logger, dataConfig);

            UpdateLatencys = new CircularBuffer<double>(10);
        }

        public void AddonRefresh()
        {
            int uiMapId = PlayerReader.UIMapId;
            Refresh();
            if(seq == 0 || uiMapId != PlayerReader.UIMapId)
                ZoneChanged?.Invoke(this, EventArgs.Empty);

            // 20 - 29
            BagReader.Read();

            // 30 - 31
            equipmentReader.Read();

            // 44
            ActionBarCostReader.Read();

            LevelTracker.Update();

            PlayerReader.UpdateCreatureLists();

            areaDb.Update(WorldMapAreaDb.GetAreaId(PlayerReader.UIMapId));

            seq++;

            if (PlayerReader.GlobalTime != lastGlobalTime)
            {
                UpdateLatencys.Put((DateTime.Now - lastGlobalTimeChange).TotalMilliseconds);

                lastGlobalTime = PlayerReader.GlobalTime;
                lastGlobalTimeChange = DateTime.Now;

                PlayerReader.AvgUpdateLatency = 0;
                for (int i = 0; i < UpdateLatencys.Size; i++)
                {
                    PlayerReader.AvgUpdateLatency += UpdateLatencys.PeekAt(i);
                }
                PlayerReader.AvgUpdateLatency /= UpdateLatencys.Size;
            }

            if ((DateTime.Now - lastFrontendUpdate).TotalMilliseconds >= FrontendUpdateIntervalMs)
            {
                seq = 0;
                AddonDataChanged?.Invoke(this, new EventArgs());

                lastFrontendUpdate = DateTime.Now;
            }
        }

        public void Refresh()
        {
            addonDataProvider.Update();
            PlayerReader.Updated();
        }

        public Color GetColorAt(int index)
        {
            return addonDataProvider.GetColor(index);
        }

        public void Dispose()
        {
            addonDataProvider?.Dispose();
        }
    }
}