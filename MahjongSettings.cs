using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SuzuFactory.AozoraMahjong
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MahjongSettings : UdonSharpBehaviour
    {
        public Material sunnyDaySkybox;
        public Material nightSkybox;
        public GameObject sun;
        public GameObject nightLight;

        public Transform tilesSet;
        public Material tileMaterial;
        public Material tileWithNumberMaterial;
        public Material tileUnlitMaterial;
        public Material tileWithNumberUnlitMaterial;
        public Material tileDefaultBackMaterial;
        public Material[] tileWoodGrainMaterials;
        public Material tileImageBackMaterial;

        public void OnSunnyDayButton()
        {
            RenderSettings.skybox = sunnyDaySkybox;
            sun.SetActive(true);
            nightLight.SetActive(false);
        }

        public void OnCloudyDayButton()
        {
            RenderSettings.skybox = sunnyDaySkybox;
            sun.SetActive(false);
            nightLight.SetActive(false);
        }

        public void OnNightButton()
        {
            RenderSettings.skybox = nightSkybox;
            sun.SetActive(false);
            nightLight.SetActive(true);
        }

        public void OnNormalTilesButton()
        {
            SetTileMaterial(tileMaterial);
        }

        public void OnNormalTilesUnlitButton()
        {
            SetTileMaterial(tileUnlitMaterial);
        }

        public void OnTilesWithNumbersButton()
        {
            SetTileMaterial(tileWithNumberMaterial);
        }

        public void OnTilesWithNumbersUnlitButton()
        {
            SetTileMaterial(tileWithNumberUnlitMaterial);
        }

        public void OnTilesWithStandardBackButton()
        {
            SetTileBackMaterial(tileDefaultBackMaterial);
        }

        public void OnTilesWithImageBackButton()
        {
            SetTileBackMaterial(tileImageBackMaterial);
        }

        public void OnTilesWithWoodgrainBackButton()
        {
            SetTileBackMaterialUnique(tileWoodGrainMaterials);
        }

        private void SetTileMaterial(Material material)
        {
            for (int i = 0; i < tilesSet.childCount; ++i)
            {
                var tiles = tilesSet.GetChild(i);

                for (int j = 0; j < tiles.childCount; ++j)
                {
                    var tile = tiles.GetChild(j);
                    var meshRenderer = tile.GetComponent<MeshRenderer>();
                    var materials = meshRenderer.sharedMaterials;
                    materials[1] = material;
                    meshRenderer.materials = materials;
                }
            }
        }

        private void SetTileBackMaterial(Material material)
        {
            for (int i = 0; i < tilesSet.childCount; ++i)
            {
                var tiles = tilesSet.GetChild(i);

                for (int j = 0; j < tiles.childCount; ++j)
                {
                    var tile = tiles.GetChild(j);
                    var meshRenderer = tile.GetComponent<MeshRenderer>();
                    var materials = meshRenderer.sharedMaterials;
                    materials[0] = material;
                    meshRenderer.materials = materials;
                }
            }
        }

        private void SetTileBackMaterialUnique(Material[] materialsSet)
        {
            int materialsAmount = materialsSet.Length;
            for (int i = 0; i < tilesSet.childCount; ++i)
            {
                var tiles = tilesSet.GetChild(i);
                int materialIndex = i % materialsAmount;

                for (int j = 0; j < tiles.childCount; ++j)
                {
                    var tile = tiles.GetChild(j);
                    var meshRenderer = tile.GetComponent<MeshRenderer>();
                    var materials = meshRenderer.sharedMaterials;
                    materials[0] = materialsSet[materialIndex];
                    meshRenderer.materials = materials;
                }
            }
        }
    }
}
