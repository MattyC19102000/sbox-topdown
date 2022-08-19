namespace Sandbox
{
	/*
		Camera which follows pawn from the top of the screen

		ToDo:
		* add screen space cursor
		* follow cursor and keep pawn in screen at the same time
	*/
	public partial class TopDownCamera : CameraMode
	{
		[Net]
		 protected float CameraHeight { get; set; } = 350.0f;

		[Net, Predicted]
		 public Vector3 MouseWorldPos { get; set; }

		// Called when the camera is first activated
		public override void Activated()
		{
			// get the pawn the camera is for
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			// set FOV
			FieldOfView = 90;

			// Look straight down
			Rotation = Rotation.From(90, 0, 0);

		}

		public virtual void FollowPawn(Entity pawn)
		{
			// keep the camera on top of the pawn
            Position = pawn.Position + (Vector3.Up * CameraHeight);
		}

		public virtual void GetMouseWorldSpace()
		{
			// old input.cursor code
			/*var tr = Trace.Ray( Input.Cursor.Origin, Input.Cursor.Origin + Input.Cursor.Direction * 100000)
			.WithoutTags( "player" )
			.WithAnyTags("solid")
			.Run();*/
			

			var direction = Screen.GetDirection(new Vector2(Mouse.Position.x, Mouse.Position.y), FieldOfView, Rotation, Screen.Size);
			var tr = Trace.Ray(Position, Position + direction * 1000)
			.UseHitboxes()
			.WithoutTags("player")
			.WithAnyTags("solid")
			.Run();

			if(tr.Hit)
			{
				DebugOverlay.TraceResult( tr );
				MouseWorldPos = tr.HitPosition;
			}
			else
			{
				Log.Info("Mouse Failed to Hit World Space");
			}
		}

		public virtual void SetPawnRotation()
		{
			var pawn = Local.Pawn;
			if( pawn == null) return;
			pawn.Rotation = Rotation.LookAt((MouseWorldPos - pawn.Position).WithZ(0), Vector3.Up);
		}

		// called in Simulate()
		public override void Update()
		{
			// grab pawn again
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			FollowPawn(pawn);
			GetMouseWorldSpace();
			SetPawnRotation();
		}
	}
}
