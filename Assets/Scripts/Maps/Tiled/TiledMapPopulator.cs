﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tiled.Net.Layers;
using Tiled.Net.Maps;
using Tiled.Net.Tilesets;
using UnityEngine;

namespace Assets.Scripts.Maps.Tiled
{
    public class TiledMapPopulator
    {
        #region Fields
        private readonly string _resourceDirectory;
        private readonly Action<GameObject, TilesetTileResource> _transformTile;
        private readonly Action<GameObject, ITiledMapObject> _transformMapObject;
        #endregion

        #region Constructors
        public TiledMapPopulator(
            string resourceDirectory,
            Action<GameObject, TilesetTileResource> transformTile,
            Action<GameObject, ITiledMapObject> transformMapObject)
        {
            _resourceDirectory = resourceDirectory;
            _transformTile = transformTile;
            _transformMapObject = transformMapObject;
        }
        #endregion

        #region Properties
        public float SpriteSpacingMultiplier { get; set; }

        public float SpriteScaleMultiplier { get; set; }

        public bool FlipVerticalPlacement { get; set; }

        public bool FlipHorizontalPlacement { get; set; }

        public bool CenterAlignGameObjects { get; set; }
        #endregion

        #region Methods
        public void PopulateMap(GameObject mapObject, ITiledMap map)
        {
            var resources = BuildResources(map.Tilesets, "Assets/Resources/Maps/");

            var tilesContainer = new GameObject()
            {
                name = "Tiles",
            };
            tilesContainer.transform.parent = mapObject.transform;

            Debug.Log("Map Layers: " + map.Layers.Count());
            foreach (var layer in map.Layers)
            {
                var layerContainer = new GameObject();
                layerContainer.transform.parent = tilesContainer.transform;
                layerContainer.name = layer.Name;

                Debug.Log("Layer Size: " + layer.Width + "x" + layer.Height);
                for (int columnIndex = 0; columnIndex < layer.Width; columnIndex++)
                {
                    for (int rowIndex = 0; rowIndex < layer.Height; rowIndex++)
                    {
                        var tile = layer.GetTile(columnIndex, rowIndex);
                        if (tile == null || tile.Gid == 0)
                        {
                            continue;
                        }

                        var tileResource = resources[tile.Gid];

                        var tileObject = new GameObject();
                        tileObject.transform.parent = layerContainer.transform;
                        tileObject.AddComponent<SpriteRenderer>();

                        var renderer = tileObject.GetComponent<SpriteRenderer>();
                        renderer.sprite = tileResource.Sprite;
                        renderer.transform.localScale *= SpriteScaleMultiplier;
                        renderer.transform.Translate(
                            columnIndex * SpriteSpacingMultiplier * (FlipHorizontalPlacement ? -1 : 1),
                            rowIndex * SpriteSpacingMultiplier * (FlipVerticalPlacement ? -1 : 1),
                            0);
                        renderer.sortingLayerName = layerContainer.name;

                        tileObject.name = renderer.sprite.name;

                        if (_transformTile != null)
                        {
                            _transformTile(tileObject, tileResource);
                        }
                    }
                }
            }

            Debug.Log("Object Layers: " + map.ObjectLayers.Count());
            foreach (var objectLayer in map.ObjectLayers)
            {
                var objectLayerContainer = new GameObject();
                objectLayerContainer.transform.parent = tilesContainer.transform;
                objectLayerContainer.name = objectLayer.Name;

                foreach (var objectLayerObject in objectLayer.Objects)
                {
                    var positionX = objectLayerObject.X / map.TileWidth * SpriteSpacingMultiplier * (FlipHorizontalPlacement ? -1 : 1);
                    var positionY = objectLayerObject.Y / map.TileHeight * SpriteSpacingMultiplier * (FlipVerticalPlacement ? -1 : 1);

                    var originalPrefab = Resources.Load(objectLayerObject.Type);
                    if (originalPrefab == null)
                    {
                        Debug.LogWarning(string.Format("Prefab does not exist at '{0}'.", objectLayerObject.Type));
                        continue;
                    }

                    var prefab = (GameObject)UnityEngine.Object.Instantiate(
                        originalPrefab,
                        new Vector3(
                            positionX,
                            positionY,
                            0),
                        Quaternion.identity);
                    prefab.transform.parent = objectLayerContainer.transform;
                    prefab.name = objectLayerObject.Name;

                    var width = objectLayerObject.Width.HasValue
                        ? objectLayerObject.Width.Value / map.TileWidth * SpriteScaleMultiplier
                        : SpriteScaleMultiplier;
                    var height = objectLayerObject.Height.HasValue
                        ? objectLayerObject.Height.Value / map.TileHeight * SpriteScaleMultiplier
                        : -SpriteScaleMultiplier; // FIXME: comment on why this default height needs to be negative for things to align properly
                    
                    prefab.transform.localScale = new Vector3(
                        width,
                        height,
                        prefab.transform.localScale.z);

                    if (CenterAlignGameObjects)
                    {
                        prefab.transform.Translate(
                            (width / 2) * (FlipHorizontalPlacement ? -1 : 1),
                            (height / 2) * (FlipVerticalPlacement ? -1 : 1),
                            0);
                    }

                    var renderer = prefab.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        renderer.sortingLayerName = objectLayerContainer.name;
                    }

                    if (_transformMapObject != null)
                    {
                        _transformMapObject(prefab, objectLayerObject);
                    }
                }
            }
        }

        private Dictionary<int, TilesetTileResource> BuildResources(IEnumerable<ITileset> tilesets, string mapResourceRoot)
        {
            var resources = new Dictionary<int, TilesetTileResource>();

            foreach (var tileset in tilesets)
            {
                int gid = tileset.FirstGid;

                var tilesetImage = tileset.Images.First();
                var resourcePath = ResourcePathFromSourcePath(mapResourceRoot, tilesetImage.SourcePath);
                Debug.Log("Resource at: " + resourcePath);

                var sprites = Resources.LoadAll<Sprite>(resourcePath);

                Debug.Log("Tile count: " + tileset.Tiles.Count());
                foreach (var tilesetTile in tileset.Tiles)
                {
                    Debug.Log("Resource for GID: " + gid);
                    resources[gid++] = new TilesetTileResource(
                        sprites[tilesetTile.Id],
                        tilesetTile.Properties);
                }
            }

            return resources;
        }

        private string ResourcePathFromSourcePath(string mapResourceRoot, string tilesetImageSourcePath)
        {
            var resourcePath = CollapsePath(mapResourceRoot + tilesetImageSourcePath);
            resourcePath = resourcePath.Substring(resourcePath.IndexOf(_resourceDirectory, StringComparison.OrdinalIgnoreCase) + _resourceDirectory.Length);

            var lastPeriodIndex = resourcePath.LastIndexOf(".", StringComparison.Ordinal);
            if (lastPeriodIndex != -1)
            {
                resourcePath = resourcePath.Substring(0, lastPeriodIndex);
            }

            return resourcePath;
        }

        private string CollapsePath(string path)
        {
            var pathSegments = path.Split('/');
            var builder = new StringBuilder();
            string lastSegment = null;

            foreach (var segment in pathSegments)
            {
                if (segment == "..")
                {
                    lastSegment = null;
                    continue;
                }

                if (lastSegment != null)
                {
                    builder.Append(lastSegment + "/");   
                }

                lastSegment = segment;
            }

            if (lastSegment != null)
            {
                builder.Append(lastSegment);
            }

            return builder.ToString();
        }
        #endregion
    }
}
