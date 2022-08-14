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

		[Net]
		 protected float WalkSpeed { get; set; } = 250.0f;

		public TopDownController()
		{
			
		}

		// Make the character move
		public virtual void TopDownWalk()
		{
			// Get current inputs to create direction vector
			Vector3 direction = new Vector3(Input.Forward, Input.Left, 0);
			// normalize the direction vector and multiply by walkspeed to get world space direction
			direction = direction.Normal * WalkSpeed;
			// apply the movement
			Position += direction * Time.Delta;
		}

		public override void Simulate()
		{
			TopDownWalk();
		}

		public override void FrameSimulate()
		{
			base.FrameSimulate();
		}

	}
}