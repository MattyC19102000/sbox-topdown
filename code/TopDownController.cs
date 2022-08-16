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
		[Net] public float Gravity { get; set; } = 400.0f;

		public TopDownController()
		{
			
		}

		// Return speed based on which buttons are pressed
		public virtual float GetSpeed()
		{
			if (Input.Down(InputButton.Run)) return RunSpeed;
			if (Input.Down(InputButton.Walk)) return WalkSpeed;

			return Speed;
		}

		// Make the character move
		public virtual void TopDownWalk()
		{
			// Get current inputs to create direction vector
			Vector3 direction = new Vector3(Input.Forward, Input.Left, 0);
			// normalize the direction vector and multiply by walkspeed to get world space direction
			direction = direction.Normal * Speed;
			// apply the movement
			Position += direction * Time.Delta;
		}

		public override void Simulate()
		{
			// apply gravity
			Velocity -= new Vector3(0, 0, Gravity) * Time.Delta;

			TopDownWalk();

			// if standing on the ground
			if(GroundEntity != null) 
			{
				Velocity = Velocity.WithZ(0);
			}
		}

		public override void FrameSimulate()
		{
			base.FrameSimulate();
		}

	}
}