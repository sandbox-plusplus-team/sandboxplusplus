namespace Sandbox.Tools
{
	[Library( "tool_terryballoon", Title = "Colored Terry Deployer", Description = "Deploy a Colored Terry! (Left click to have it attached to a beam. Right click to not.)", Group = "construction" )]
	public partial class TerryBalloonTool : BaseTool
	{
		[Net]
		public Color Tint { get; set; }

		PreviewEntity previewModel;

		public override void Activate()
		{
			base.Activate();

			if ( Host.IsServer )
			{
				Tint = Color.Random;
			}
		}

		protected override bool IsPreviewTraceValid( TraceResult tr )
		{
			if ( !base.IsPreviewTraceValid( tr ) )
				return false;

			return true;
		}

		public override void CreatePreviews()
		{
			if ( TryCreatePreview( ref previewModel, "models/terrybaloon.vmdl") )
			{
				previewModel.RelativeToNormal = false;
			}
		}

		public override void Simulate()
		{
			if ( previewModel.IsValid() )
			{
				previewModel.RenderColor = Tint;
			}

			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				bool useRope = Input.Pressed( InputButton.Attack1 );
				if ( !useRope && !Input.Pressed( InputButton.Attack2 ) )
					return;

				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();

				if ( !tr.Hit )
					return;

				if ( !tr.Entity.IsValid() )
					return;

				CreateHitEffects( tr.EndPos );

				if ( tr.Entity is BalloonEntity )
					return;

				var ent = new BalloonEntity
				{
					Position = tr.EndPos,
				};

				ent.SetModel("models/terrybaloon.vmdl");
				ent.PhysicsBody.GravityScale = -50000000000000000000000.2f;
				ent.RenderColor = Tint;

				Tint = Color.Random;

				if ( !useRope )
					return;

				var rope = Particles.Create("particles/physgun_beam.vpcf");
				rope.SetEntity( 0, ent );

				var attachEnt = tr.Body.IsValid() ? tr.Body.Entity : tr.Entity;
				var attachLocalPos = tr.Body.Transform.PointToLocal( tr.EndPos ) * (1.0f / tr.Entity.Scale);

				if ( attachEnt.IsWorld )
				{
					rope.SetPosition( 1, attachLocalPos );
				}
				else
				{
					rope.SetEntityBone( 1, attachEnt, tr.Bone, new Transform( attachLocalPos ) );
				}

				var spring = PhysicsJoint.Spring
					.From( ent.PhysicsBody )
					.To( tr.Body, tr.Body.Transform.PointToLocal( tr.EndPos ) )
					.WithFrequency( 5.0f )
					.WithDampingRatio( 0.7f )
					.WithReferenceMass( ent.PhysicsBody.Mass )
					.WithMinRestLength( 0 )
					.WithMaxRestLength( 100 )
					.WithCollisionsEnabled()
					.Create();

				spring.EnableAngularConstraint = false;
				spring.OnBreak( () =>
				{
					rope?.Destroy( true );
					spring.Remove();
				} );

				var player = Owner as SandboxPlayer;
				player.AddToUndo( ent );
			}
		}
	}
}
