using System;

namespace Sandbox{

	/*
		Controller which moves the pawn within pseudo 2d top down space

		ToDo:
		Fix weird velocity slingshotting
		add primitive shooting

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
		[Net] public float Acceleration { get; set; } = 10.0f;
		[Net] public float Friction { get; set; } = 250.0f;

		public Unstuck Unstuck;

		public WeaponBase Weapon;

		public TopDownController()
		{
			Unstuck = new Unstuck(this);
			Weapon = new DebugWeapon(this);
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
			// Gather Input for Movement
			WishVelocity = new Vector3(Input.Forward, Input.Left, 0);
			// Multiply Input Unit Vector to give Magnitude
			WishVelocity *= GetSpeed();
			// Clamp the overall length at speed to prevent "Strafe Walking"
			WishVelocity = WishVelocity.ClampLength(GetSpeed());

			// Clamp Z Velocity before and after accelerate to ensure pawn doesn't lift off floor
			Velocity = Velocity.WithZ(0);
			ApplyAccelerate();
			Velocity = Velocity.WithZ(0);

			// if velocity is near zero, set it to zero
			if (Velocity.Length < 1.0f)
			{
				Velocity = Vector3.Zero;
				// return since no position change is necessary
				return;
			}

			// Get the position where the pawn will be
			var tryMove = (Position + Velocity * Time.Delta).WithZ(Position.z);
			// Trace a bounding box to this location
			var pm = TraceBBox(Position, tryMove);
			// if the trace hits no solid objects
			if(pm.Fraction == 1)
			{
				// allow the pawn to move to the desired position
				Position = pm.EndPosition;
			}
		}

		public virtual void ApplyAccelerate()
		{
			// gather unit vector for direction
			var direction = WishVelocity.Normal;
			// get "speed" from magnitude
			var speed = WishVelocity.Length;
			// calculate the speed of the acceleration
			var accelSpeed = Acceleration * Time.Delta * speed;
			// apply acceleration to the velocity
			Velocity += accelSpeed * direction;

			Velocity = Velocity.ClampLength(GetSpeed());
		}

		public virtual void ApplyFriction()
		{
			// get speed of vector
			var speed = Velocity.Length;

			// if the speed is too small don't bother applying friction
			if(speed < 0.1f) return;

			var frictionMod = (speed < Friction) ? Friction : speed;

			var frictionSpeed = frictionMod * Time.Delta;

			float newspeed = speed - frictionSpeed;
            if (newspeed < 0) newspeed = 0;

			if (newspeed != speed)
            {
                newspeed /= speed;
                Velocity *= newspeed;
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

			// Use Unstuck class to test if pawn is Stuck
			if (Unstuck.TestAndFix())
                return;

			// Check for floor against bounding box
			GetGroundEntity(TraceBBox(Position, Position + Vector3.Down * 2));

			bool onGround = GroundEntity != null;
			// Stop pawn from phasing through the floor with gravity
			if(onGround)
			{
				ApplyFriction();
				Move();
			}
			else
			{
				// if the pawn is in the air they will fall
				Fall();
			}

			if(Input.Pressed(InputButton.PrimaryAttack))
			{
				Weapon.Fire();
			}

		}

		public override void FrameSimulate()
		{
			base.FrameSimulate();
		}

	}
}