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
        //private string Binding;

        public void Start ()
        {
            Debug.Log("InflightShipSave.Start()");

            //ConfigNode CNBinding = new ConfigNode("Game");
            //CNBinding = GameDatabase.Instance.GetConfigNode("INFLIGHT_SHIP_SAVE");

            //Binding = CNBinding.GetValue("primary");
            //Binding = (GameDatabase.Instance.GetConfigNode("INFLIGHT_SHIP_SAVE")).GetValue("primary");
        }

        public void Update ()
        {
            // Debug.LogWarning("InflightShipSave.Update()");

            if (Input.GetKeyDown(KeyCode.F6))
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
                CleanNodes(CN);

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

        private void CleanNodes (ConfigNode CN)
        {
            if (null == CN) { return; }

            CN.SetValue("EngineIgnited", "False");
            CN.SetValue("currentThrottle", "0");
            CN.SetValue("Staged", "False");
            CN.SetValue("sensorActive", "False");
            CN.SetValue("active", "False");
            CN.SetValue("throttle", "0");
            CN.SetValue("generatorIsActive", "False");

            for(int IndexNodes = 0; IndexNodes < CN.nodes.Count; IndexNodes++)
            {
                CleanNodes(CN.nodes[IndexNodes]);
            }
        }
        public void OnDestroy ()
        {
            Debug.Log("InflightShipSave.OnDestroy()");
        }
    }
}
