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

		// Networked pawn variables
		[Net] public float Speed { get; set; } = 250.0f;
		[Net] public float WalkSpeed { get; set; } = 150.0f;
		[Net] public float RunSpeed { get; set; } = 400.0f;
		[Net] public float Gravity { get; set; } = 800.0f;
		[Net] public float BodyGirth { get; set; } = 32.0f;
        [Net] public float BodyHeight { get; set; } = 72.0f;

		public TopDownController()
		{
			
		}

		// Client Bounding Box information
		protected Vector3 mins;
        protected Vector3 maxs;

		// Set the information of the pawns bounding box
		public virtual void SetBBox(Vector3 mins, Vector3 maxs)
        {
            if (this.mins == mins && this.maxs == maxs)
                return;

            this.mins = mins;
            this.maxs = maxs;
        }

		// Change the pawns bounding box
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

		// Get the entity information of whatever is below the pawn
		public virtual void GetGroundEntity(TraceResult tr)
		{
			GroundEntity = tr.Entity;
		}

		// Movement function for when the pawn is on the ground
		public virtual void Move()
		{
			// Get input from the user
			Velocity = new Vector3(Input.Forward, Input.Left, 0); 
			// Change the speed of the pawn depending on their chosen movement speed
			Velocity *= GetSpeed();
			// Ensure pawn stays on floor
			Velocity = Velocity.WithZ(0);
			
			// Get where the pawn will be after velocity change
			var dest = (Position + Velocity * Time.Delta).WithZ(Position.z);
			// trace the bounding to this destination
			var premove = TraceBBox(Position, dest);
			// if the box doesn't collide allow the pawn to move
			if(premove.Fraction == 1)
			{
				Position = premove.EndPosition;
				return;
			}
		}

		// Allow the pawn to be affected by gravity
		public virtual void Fall()
		{
			Position += Velocity * Time.Delta;
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
				// if the pawn is in the air they will fall
				Fall();
			}
		}

		public override void FrameSimulate()
		{
			base.FrameSimulate();
		}

	}
}