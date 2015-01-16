/*
 * This module written by Claw. For more details please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules 
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 * Written for KSP v0.90.0
 *
 * InflightShipSave v0.1.0
 * 
 * This plugin allows the user to save the active, in-flight vessel back out to a .craft file.
 * 
 * Things to fix:
 * - Make the save key bindable (so that it can be changed in a .cfg file)
 * - Adjust the rotation/position of the root part, so that the craft isn't sideways in the ground.
 * 
 * 
 * Change Log:
 * 
 * v0.1.0 (13 Jan 15) - Initial release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class InflightShipSave : MonoBehaviour
    {
        private KeyCode BoundKey = KeyCode.F6;

        public void Start ()
        {
            Debug.Log("InflightShipSave.Start()");

            if (null == GameDatabase.Instance.GetConfigNodes("INFLIGHT_SHIP_SAVE")) { return; }

            ConfigNode CNBinding = new ConfigNode();
            CNBinding = GameDatabase.Instance.GetConfigNodes("INFLIGHT_SHIP_SAVE")[0];

            if (null != CNBinding)
            {
                string BindingString = CNBinding.GetValue("primary");
                if (!string.IsNullOrEmpty(BindingString))
                {
                    KeyCode BoundKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), BindingString);
                    if (KeyCode.None == BoundKey)
                    {
                        BoundKey = KeyCode.F6;
                    }
                }
            }
        }

        public void Update ()
        {
            // Debug.LogWarning("InflightShipSave.Update()");

            if (Input.GetKeyDown(BoundKey))
            {
                Vessel VesselToSave = FlightGlobals.ActiveVessel;
                string ShipName = VesselToSave.vesselName;
                Debug.LogWarning("Saving: " + ShipName);

                ShipConstruct ConstructToSave = new ShipConstruct(ShipName + "_Rescued", "TempDescription", VesselToSave.parts[0]);

                Quaternion OriginalRotation = VesselToSave.vesselTransform.rotation;
                Vector3 OriginalPosition = VesselToSave.vesselTransform.position;

                VesselToSave.SetRotation(new Quaternion(0, 0, 0, 1));
                Vector3 ShipSize = ShipConstruction.CalculateCraftSize(ConstructToSave);
                VesselToSave.SetPosition(new Vector3(0, ShipSize.y+2, 0));

                ConfigNode CN = new ConfigNode("ShipConstruct");
                CN = ConstructToSave.SaveShip();
                CleanEditorNodes(CN);

                VesselToSave.SetRotation(OriginalRotation);
                VesselToSave.SetPosition(OriginalPosition);

                Debug.LogWarning("Facility: " + ConstructToSave.shipFacility);

                if (ConstructToSave.shipFacility == EditorFacility.SPH)
                {
                    Debug.LogWarning("Ship Saved: " + UrlDir.ApplicationRootPath + "saves/" + HighLogic.SaveFolder
                        + "/Ships/SPH/" + ShipName + "_Rescued.craft");
                    CN.Save(UrlDir.ApplicationRootPath + "saves/" + HighLogic.SaveFolder
                        + "/Ships/SPH/" + ShipName + "_Rescued.craft");
                }
                else
                {
                    Debug.LogWarning("Ship Saved: " + UrlDir.ApplicationRootPath + "saves/" + HighLogic.SaveFolder
                        + "/Ships/VAB/" + ShipName + "_Rescued.craft");
                    CN.Save(UrlDir.ApplicationRootPath + "saves/" + HighLogic.SaveFolder
                        + "/Ships/VAB/" + ShipName + "_Rescued.craft");
                }
            }     
        }

        private void CleanEditorNodes (ConfigNode CN)
        {

            CN.SetValue("EngineIgnited", "False");
            CN.SetValue("currentThrottle", "0");
            CN.SetValue("Staged", "False");
            CN.SetValue("sensorActive", "False");
            CN.SetValue("throttle", "0");
            CN.SetValue("generatorIsActive", "False");
            CN.SetValue("persistentState", "STOWED");

            string ModuleName = CN.GetValue("name");

            // Turn off or remove specific things
            if ("ModuleScienceExperiment" == ModuleName)
            {
                CN.RemoveNodes("ScienceData");
            }
            else if ("ModuleScienceExperiment" == ModuleName)
            {
                CN.SetValue("Inoperable", "False");
                CN.RemoveNodes("ScienceData");
            }
            else if ("Log" == ModuleName)
            {
                CN.ClearValues();
            }


            for (int IndexNodes = 0; IndexNodes < CN.nodes.Count; IndexNodes++)
            {
                CleanEditorNodes (CN.nodes[IndexNodes]);
            }
        }

        private void PristineNodes (ConfigNode CN)
        {
            if (null == CN) { return; }

            if ("PART" == CN.name)
            {
                string PartName = ((CN.GetValue("part")).Split('_'))[0];

                Debug.LogWarning("PART: " + PartName);
                
                Part NewPart = PartLoader.getPartInfoByName(PartName).partPrefab;
                ConfigNode NewPartCN = new ConfigNode();
                Debug.LogWarning("New Part: " + NewPart.name);

                NewPart.InitializeModules();

                CN.ClearNodes();

                // EVENTS, ACTIONS, PARTDATA, MODULE, RESOURCE

                Debug.LogWarning("EVENTS");
                NewPart.Events.OnSave(CN.AddNode("EVENTS"));
                Debug.LogWarning("ACTIONS");
                NewPart.Actions.OnSave(CN.AddNode("ACTIONS"));
                Debug.LogWarning("PARTDATA");
                NewPart.OnSave(CN.AddNode("PARTDATA"));
                Debug.LogWarning("MODULE");
                for (int IndexModules = 0; IndexModules < NewPart.Modules.Count; IndexModules++)
                {
                    NewPart.Modules[IndexModules].Save(CN.AddNode("MODULE"));
                }
                Debug.LogWarning("RESOURCE");
                for (int IndexResources = 0; IndexResources < NewPart.Resources.Count; IndexResources++)
                {
                    NewPart.Resources[IndexResources].Save(CN.AddNode("RESOURCE"));
                }

                //CN.AddNode(CompiledNodes);

                return;
            }
            for (int IndexNodes = 0; IndexNodes < CN.nodes.Count; IndexNodes++)
            {
                PristineNodes(CN.nodes[IndexNodes]);
            }
        }

        public void OnDestroy ()
        {
            Debug.Log("InflightShipSave.OnDestroy()");
        }
    }
}
