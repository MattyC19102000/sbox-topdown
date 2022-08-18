namespace Sandbox{

	/*
		Controller which moves the pawn within pseudo 2d top down space

		ToDo:
		Make pawn rotate in world space
		Make pawn look at mouse cursor
		Add collisions

	*/
	public partial class TopDownController : BasePlayerController
	{

		[Net] public float Speed { get; set; } = 250.0f;
		[Net] public float WalkSpeed { get; set; } = 150.0f;
		[Net] public float RunSpeed { get; set; } = 400.0f;
		[Net] public float Gravity { get; set; } = 800.0f;
		[Net] public float BodyGirth { get; set; } = 32.0f;
        [Net] public float BodyHeight { get; set; } = 72.0f;
		public TopDownController()
		{
			
		}

		protected Vector3 mins;
        protected Vector3 maxs;

		public virtual void SetBBox(Vector3 mins, Vector3 maxs)
        {
            if (this.mins == mins && this.maxs == maxs)
                return;

            this.mins = mins;
            this.maxs = maxs;
        }

        public virtual void UpdateBBox()
        {
            var girth = BodyGirth * 0.5f;

            var mins = new Vector3(-girth, -girth, 0) * Pawn.Scale;
            var maxs = new Vector3(+girth, +girth, BodyHeight) * Pawn.Scale;

            SetBBox(mins, maxs);
        }

		// Return speed based on which buttons are pressed
		public virtual float GetSpeed()
		{
			if (Input.Down(InputButton.Run)) return RunSpeed;
			if (Input.Down(InputButton.Walk)) return WalkSpeed;

			return Speed;
		}

		public virtual void GetGroundEntity(TraceResult tr)
		{
			GroundEntity = tr.Entity;
		}

		public virtual void Move()
		{
			Velocity = new Vector3(Input.Forward, Input.Left, 0);
			Velocity *= GetSpeed();
			Velocity = Velocity.WithZ(0);
			
			var dest = (Position + Velocity * Time.Delta).WithZ(Position.z);
			var premove = TraceBBox(Position, dest);
			if(premove.Fraction == 1)
			{
				Position = premove.EndPosition;
				return;
			}
		}

		public override void Simulate()
		{
			// Apply Gravity
			Velocity -= new Vector3(0, 0, Gravity) * Time.Delta;

			// Scale the bounding box for the pawn
			UpdateBBox();

			// Check for floor against bounding box
			GetGroundEntity(TraceBBox(Position, Position + Vector3.Down * 2));

			bool onGround = GroundEntity != null;
			// Stop pawn from phasing through the floor with gravity
			if(onGround)
			{
				// Finalize Movement
				Move();
			}
			else
			{
				Position += Velocity * Time.Delta;
			}
		}

		public override void FrameSimulate()
		{
			base.FrameSimulate();
		}

	}
}