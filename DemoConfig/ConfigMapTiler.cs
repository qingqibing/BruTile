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
using BruTile;
using BruTile.FileSystem;
using BruTile.Cache;

namespace DemoConfig
{
    public class ConfigMapTiler : IConfig
    {
        public ITileSource CreateTileSource()
        {
            return new TileSource(TileProvider, TileSchema);
        }

        public static ITileProvider TileProvider
        {
            get
            {
                return new FileTileProvider(new FileCache(GetAppDir() + "\\Resources\\GeoData\\TrueMarble", "png"));
            }
        }

        public static ITileSchema TileSchema
        {
            get
            {
                double[] resolutions = new double[] { 156543.033900000, 78271.516950000 };

                TileSchema schema = new TileSchema();
                foreach (double resolution in resolutions) schema.Resolutions.Add(resolution);
                schema.Height = 256;
                schema.Width = 256;
                schema.Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
                schema.OriginX = -20037508.342789;
                schema.OriginY = -20037508.342789;
                schema.Name = "OpenStreetMap";
                schema.Format = "png";
                schema.Srs = "EPSG:3785";
                return schema;
            }
        }

        private static string GetAppDir()
        {
            return System.IO.Path.GetDirectoryName(
              System.Reflection.Assembly.GetEntryAssembly().GetModules()[0].FullyQualifiedName);
        }
    }
}
