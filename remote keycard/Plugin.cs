﻿using Qurre.API.Events;
using System.Linq;
namespace remote_keycard
{
	public class Plugin : Qurre.Plugin
	{
		#region override
		public override System.Version Version => new System.Version(1, 0, 5); 
		public override System.Version NeededQurreVersion => new System.Version(1, 5, 0);
		public override string Developer => "fydne";
		public override string Name => "remote keycard";
		public override void Enable() => RegisterEvents();
		public override void Disable() => UnregisterEvents();
		#endregion
		#region events
		private void RegisterEvents()
		{
			Qurre.Events.Player.InteractDoor += RunOnDoorOpen;
			Qurre.Events.Player.InteractLocker += LockerInteraction;
			Qurre.Events.Player.InteractGenerator += GenOpen;
		}
		private void UnregisterEvents()
		{
			Qurre.Events.Player.InteractDoor -= RunOnDoorOpen;
			Qurre.Events.Player.InteractLocker -= LockerInteraction;
			Qurre.Events.Player.InteractGenerator -= GenOpen;
		}
		#endregion
		#region main
		public void RunOnDoorOpen(InteractDoorEvent ev)
		{
			if (ev.Player.Team == Team.SCP) return;
			if (!ev.Allowed)
			{
				var playerIntentory = ev.Player.Inventory.items;
				foreach (var item in playerIntentory)
				{
					var gameItem = UnityEngine.Object.FindObjectOfType<Inventory>().availableItems.FirstOrDefault(i => i.id == item.id);
					if (gameItem == null)
						continue;
					if (ev.Door.Permissions.CheckPermissions(gameItem, ev.Player.ReferenceHub))
					{
						ev.Allowed = true;
					}
				}
			}
		}
		public void LockerInteraction(InteractLockerEvent ev)
		{
			if (!ev.Allowed)
			{
				if (ev.Player.Team == Team.SCP) return;
				var playerIntentory = ev.Player.Inventory.items;
				bool chcb = false;
				bool lvl2per = false;
				foreach (var item in playerIntentory)
				{
					var gameItem = UnityEngine.Object.FindObjectOfType<Inventory>().availableItems.FirstOrDefault(i => i.id == item.id);
					if (gameItem == null)
						continue;
					if (gameItem.permissions == null || gameItem.permissions.Length == 0)
						continue;
					foreach (var itemPerm in gameItem.permissions)
					{
						if (itemPerm == "PEDESTAL_ACC")
						{
							ev.Allowed = true;
							continue;
						}
						if (itemPerm == "CHCKPOINT_ACC")
						{
							chcb = true;
						}
						if (itemPerm == "CONT_LVL_2")
						{
							lvl2per = true;
						}
					}
					if (chcb && lvl2per)
					{
						ev.Allowed = true;
						continue;
					}
				}
			}
		}
		public void GenOpen(InteractGeneratorEvent ev)
		{
			if (ev.Player.Team == Team.SCP) return;
			var playerIntentory = ev.Player.Inventory.items;
			foreach (var item in playerIntentory)
			{
				var gameItem = UnityEngine.Object.FindObjectOfType<Inventory>().availableItems.FirstOrDefault(i => i.id == item.id);
				if (gameItem == null)
					continue;
				if (gameItem.permissions == null || gameItem.permissions.Length == 0)
					continue;
				foreach (var itemPerm in gameItem.permissions)
				{
					if (itemPerm == "ARMORY_LVL_2")
					{
						ev.Allowed = true;
						continue;
					}
				}
			}
		}
		#endregion
	}
}
