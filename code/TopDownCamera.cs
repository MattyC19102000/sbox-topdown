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
		// Set of Euler Angles used to clamp and set axis' 
        private Angles followAngles;

		[Net]
		 protected float CameraHeight { get; set; } = 300.0f;

		[Net, Predicted]
		 public Vector3 MouseWorldPos { get; set; }

		// Called when the camera is first activated
		public override void Activated()
		{
			// get the pawn the camera is for
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			// set the camera to looks straight down
			followAngles.pitch = 90;
			// yaw is 0 to keep the top of the camera facing the north of the map
			followAngles.yaw = 0;
			// Set the rotation to these new angles
			Rotation = Rotation.From(followAngles);
			Position = pawn.Position + (Vector3.Up * CameraHeight);

			FieldOfView = 80;
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
			pawn.Rotation = Rotation.LookAt((MouseWorldPos - pawn.Position).WithZ(0) , Vector3.Up);
		}

		// called per tick clientside (i think)
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
