using ICities;
using ColossalFramework;
using UnityEngine;
using System;
using ColossalFramework.Plugins;

namespace AutomaticDeleteAbandoned
{
   public class AutoDeleteAbandonedMod : IUserMod
   {
      public string Name
      {
         get { return "Automatic Delete Abandoned"; }
      }

      public string Description
      {
         get
         {
            return
               "This mod automatically deletes abandoned buildings immediately after becoming abandoned.";
         }
      }
   }

   public class AutoDeleteAbandonedThread : ThreadingExtensionBase
   {
      private BuildingManager _buildingManager;
      //private FastList<ushort> _abandonedBuildings;
      private float timer = 0f;

      public override void OnCreated(IThreading threading)
      {
         
      }

      public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
      {
         if (timer > 0.5f)
         {
            timer = 0f;
            _buildingManager = Singleton<BuildingManager>.instance;
            var buildings = _buildingManager.m_buildings.m_buffer;

            for (var i = 0; i < _buildingManager.m_buildings.m_buffer.Length; i++)
            {
               var building = buildings[i];

               try
               {
                  if (((building.m_flags & Building.Flags.Created) != Building.Flags.None)
                      && ((building.m_flags & Building.Flags.Deleted) == Building.Flags.None)
                      && ((building.m_flags & Building.Flags.Abandoned) != Building.Flags.None) ||
                      ((building.m_flags & Building.Flags.BurnedDown) != Building.Flags.None))
                  {
                     if (building.Info.m_buildingAI.CheckBulldozing((ushort)i, ref building) ==
                         ToolBase.ToolErrors.None)
                     {
                        //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, building.m_flags.ToString());
                        _buildingManager.ReleaseBuilding((ushort)i);
                        //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Demolished abandoned building #" + (ushort)i);
                        //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, building.m_flags.ToString());
                        //break;
                     }
                  }
               }
               catch (Exception ex)
               {
                  DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, "Error deleting abandoned building #" + (ushort)i);
               }
            }

         }

         timer += simulationTimeDelta;
      }
   }
}