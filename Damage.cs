using System;


public class Damage
	{
		public float damage;
		public int penetration;

		public Damage (float dmg, int pnt)
		{
			if (dmg < 0) damage= 0; else damage = dmg;
			if (dmg < 0) penetration = 0; else penetration = pnt;
		}
	}


