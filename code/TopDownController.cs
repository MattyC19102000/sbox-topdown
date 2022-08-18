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

		public virtual void GetGroundEntity()
		{

		}

		public override void Simulate()
		{
			
			UpdateBBox();

		}

		public override void FrameSimulate()
		{
			base.FrameSimulate();
		}

	}
}