using Sandbox;

public static class SnowSurface
{
	/// <summary>
	/// Create a particle effect and play an impact sound for this surface being hit by a bullet
	/// </summary>
	public static Particles DoSnowImpact( this Surface self, TraceResult tr )
	{
		//
		// No effects on resimulate
		//
		if ( !Prediction.FirstTime )
			return null;

		//
		// Drop a decal
		//
		var decalPath = "decals/snow.decal";

		var surf = self.GetBaseSurface();
		
		while ( string.IsNullOrWhiteSpace( decalPath ) && surf != null )
		{
			decalPath = Rand.FromArray( surf.ImpactEffects.BulletDecal );
			surf = surf.GetBaseSurface();
		}

		if ( !string.IsNullOrWhiteSpace( decalPath ) )
		{
			if ( DecalDefinition.ByPath.TryGetValue( decalPath, out var decal ) )
			{
				decal.PlaceUsingTrace( tr );
			}
		}

		//
		// Make an impact sound
		//
		var sound = self.Sounds.Bullet;

		surf = self.GetBaseSurface();
		while ( string.IsNullOrWhiteSpace( sound ) && surf != null )
		{
			sound = surf.Sounds.Bullet;
			surf = surf.GetBaseSurface();
		}

		if ( !string.IsNullOrWhiteSpace( sound ) )
		{
			Sound.FromWorld( sound, tr.EndPos );
		}

		//
		// Get us a particle effect
		//

		string particleName = "particles/hit_effects/snowhit_base.vpcf";
		// if ( string.IsNullOrWhiteSpace( particleName ) ) particleName = Rand.FromArray( self.ImpactEffects.Regular );

		surf = self.GetBaseSurface();
		while ( string.IsNullOrWhiteSpace( particleName ) && surf != null )
		{
			particleName = Rand.FromArray( surf.ImpactEffects.Bullet );
			if ( string.IsNullOrWhiteSpace( particleName ) ) particleName = Rand.FromArray( surf.ImpactEffects.Regular );

			surf = surf.GetBaseSurface();
		}

		if ( !string.IsNullOrWhiteSpace( particleName ) )
		{
			var ps = Particles.Create( particleName, tr.EndPos );
			ps.SetForward( 0, tr.Normal );

			return ps;
		}

		return default;
	}
}
