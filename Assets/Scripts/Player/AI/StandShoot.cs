﻿using System;
using UnityEngine;

namespace Assets.Scripts.Player.AI
{
	/// <summary> AI that always stands still and shoots at the player. </summary>
	public class StandShoot : IPolicy
	{
		/// <summary> Timer for delay between firing. </summary>
		private float fireCooldown = 0;
		/// <summary> The delay between firing. </summary>
		private const float COOLDOWNTIME = 5;

		public void ChooseAction(AIController controller)
		{
			controller.aim = Vector3.Normalize(controller.opponent.transform.position - controller.transform.position);
			if (controller.aiming)
			{
				if (controller.ArcheryComponent.StrengthPercentage > 0.9)
				{
					controller.aiming = false;
				}
			}
			else
			{
				if (fireCooldown++ > COOLDOWNTIME)
				{
					controller.aiming = true;
					fireCooldown = 0;
				}
			}
		}
	}
}

