﻿// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.ComponentModel;
using BruTile;
using BruTile.Cache;
using BruTile.UI.Fetcher;

namespace BruTile.UI
{
    public class TileLayer<T> : IDisposable
    {
        #region Fields

        ITileSchema schema;
        TileFetcher<T> tileFetcher;
#if PocketPC
    MemoryCache<T> memoryCache = new MemoryCache<T>(40, 60);
#else
        MemoryCache<T> memoryCache = new MemoryCache<T>(100, 200);
#endif
        const int maxRetries = 3;

        #endregion

        #region EventHandlers

        public event AsyncCompletedEventHandler DataUpdated;

        #endregion

        #region Properties

        public ITileSchema Schema
        {
            //TODO: check if we need realy need this property
            get { return schema; }
        }

        public MemoryCache<T> MemoryCache
        {
            get { return memoryCache; }
        }

        #endregion

        #region Constructors

        public TileLayer(ITileSource source, ITileFactory<T> tileFactory)
        {
            this.schema = source.Schema;
            tileFetcher = new TileFetcher<T>(source, memoryCache, tileFactory);
            RegisterEventHandlers();
        }

        ~TileLayer()
        {
        }

        #endregion

        #region Public Methods

        public void UpdateData(Extent extent, double resolution)
        {
            tileFetcher.ViewChanged(extent, resolution);
        }

        #endregion

        #region Private Methods

        private void RegisterEventHandlers()
        {
            tileFetcher.DataChanged += new DataChangedEventHandler(tileFetcher_DataChanged);
        }

        private void UnRegisterEventHandlers()
        {
            tileFetcher.DataChanged -= new DataChangedEventHandler(tileFetcher_DataChanged);
        }

        private void tileFetcher_DataChanged(object sender, DataChangedEventArgs e)
        {
            OnDataUpdated(new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
        }

        private void OnDataUpdated(AsyncCompletedEventArgs e)
        {
            if (DataUpdated != null)
                DataUpdated(this, e);
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (tileFetcher != null)
            {
                UnRegisterEventHandlers();
                tileFetcher.Dispose();
            }
        }

        #endregion
    }
}
