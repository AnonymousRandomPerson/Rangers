﻿using UnityEngine;
using System;

namespace Assets.Scripts.Player.AI
{
	/// <summary>
	/// AI that shoots at the player.
	/// </summary>
	public class Shoot : IPolicy
	{
		/// <summary> Timer for delay between firing. </summary>
		private float fireCooldown = 0;
		/// <summary> The delay between firing. </summary>
		private float cooldownTime = 5;
		/// <summary> The minimum power that the AI will shoot arrows at. </summary>
		private float minPower;
		/// <summary> The minimum angle (in radians) that the AI will shoot arrows at. </summary>
		private float minAngle;

		/// <summary>
		/// Initializes a new instance of the <see cref="Assets.Scripts.Player.AI.StandShoot"/> class.
		/// </summary>
		/// <param name="cooldownTime">The delay between firing.</param>
		/// <param name="minPower">The minimum power that the AI will shoot arrows at.</param>
		/// <param name="minAngle">The minimum angle (in radians) that the AI will shoot arrows at.</param> 
		internal Shoot(float cooldownTime, float minPower, float minAngle) {
			this.cooldownTime = cooldownTime;
			this.minPower = minPower;
			this.minAngle = minAngle;
		}

		/// <summary>
		/// Picks an action for the character to do every tick.
		/// </summary>
		/// <param name="controller">The controller for the character.</param>
		public void ChooseAction(AIController controller)
		{
			if (controller.opponent.LifeComponent.Health <= 0) {
				controller.aiming = false;
				return;
			}

			Vector3 positionOffset = controller.opponent.transform.position - controller.transform.position;
			bool left = false;
			if (positionOffset.x < 0)
			{
				left = true;
				positionOffset.x = -positionOffset.x;
			}

			float angle = Vector3.Angle(Vector3.right, positionOffset) * Mathf.PI / 180;
			angle = Mathf.Max(angle, minAngle);

			float x = positionOffset.x;
			float y = positionOffset.y - 0.5f;
			float g = -Physics.gravity.y;

			// Find the minimum power for the required angle.
			float v = Mathf.Sqrt((x * x * g) / (x * Mathf.Sin(2 * angle) - 2 * y * Mathf.Pow(Mathf.Cos(angle), 2)));
			float power = (Mathf.Sqrt(10 * v + 1) - 1) / 20;

			// If the minimum power is less than the required power, find the needed angle for the power.
			if (power < minPower)
			{
				power = minPower;
				v = 40 * power * (power + 0.1f);
				float root = Mathf.Sqrt(Mathf.Pow(v, 4) - g * (g * x * x + 2 * y * v * v));
				angle = Mathf.Atan((v * v - root) / (g * x));
				if (angle < minAngle)
				{
					angle = Mathf.Atan((v * v + root) / (g * x));
				}
			}
			power = Mathf.Min(power, 0.95f);

			if (left)
			{
				angle = Mathf.PI - angle;
			}

			controller.aim = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

			if (controller.aiming)
			{
				if (controller.ArcheryComponent.StrengthPercentage >= power)
				{
					controller.ArcheryComponent.StrengthPercentage = power;
					controller.aiming = false;
				}
			}
			else
			{
				if (fireCooldown++ > cooldownTime)
				{
					controller.aiming = true;
					fireCooldown = 0;
				}
			}
		}
	}
}