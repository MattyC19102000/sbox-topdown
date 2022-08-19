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
		public float CameraHeight = 350.0f;

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

		// called in Simulate()
		public override void Update()
		{
			// grab pawn again
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			FollowPawn(pawn);
		}
	}
}
