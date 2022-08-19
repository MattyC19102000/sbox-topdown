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
		[Net] public float Speed { get; set; } = 200.0f;
		[Net] public float WalkSpeed { get; set; } = 150.0f;
		[Net] public float RunSpeed { get; set; } = 300.0f;
		[Net] public float Gravity { get; set; } = 800.0f;
		[Net] public float BodyGirth { get; set; } = 32.0f;
        [Net] public float BodyHeight { get; set; } = 72.0f;
		[Net] public float Acceleration { get; set; } = 10.0f;
		[Net] public float SlowSpeed { get; set; } = 200.0f;

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
			WishVelocity = new Vector3(Input.Forward, Input.Left, 0); 

			// Clamp Magnitude to prevent Strafe Walking
			var inSpeed = WishVelocity.Length.Clamp(0, 1);
			WishVelocity = WishVelocity.Normal * inSpeed;
			// Multiply by Desired Speed
			WishVelocity *= GetSpeed();

			// Get the Direction and Speed of the WishVelocity as seperate variables
			var wishDir = WishVelocity.Normal;
			var wishSpeed = WishVelocity.Length;

            WishVelocity = WishVelocity.Normal * wishSpeed;

			// Keep the velocity Z at 0 and apply Acceleration
			Velocity = Velocity.WithZ(0);
			ApplyAccelerate(wishDir, wishSpeed);
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

			// ensure velocity never goes above the desired speed
			Velocity = Velocity.Normal * MathF.Min( Velocity.Length, GetSpeed() );

		}

		public virtual void ApplyAccelerate(Vector3 wishDir, float wishSpeed)
		{
			// get the speed of the current wish direction vector
			var currentSpeed = Velocity.Dot(wishDir);

			// get the speed which is to be added to the velocity
			var addSpeed = wishSpeed - currentSpeed;

			// if the speed to be added is less than 0 then return
			if(addSpeed <= 0)
			{
				return;
			}

			// calculate the acceleration from the wishspeed
			var accelSpeed = Acceleration * wishSpeed * Time.Delta;

			// clamp the accelspeed to the addspeed
			if(accelSpeed > addSpeed)
			{
				accelSpeed = addSpeed;
			}
			
			// apply velocity change
			Velocity += accelSpeed * wishDir;
		}

		public virtual void ApplyFriction()
		{
			// set the harshness of the friction
			float frictionAmount = 1.0f;

			// get current speed of pawn
			var speed = Velocity.Length;

			// if not moving don't apply friction
			if(speed < 0.1f) return;

			// get the bleed amount between speed and slow speed
			float bleed = (speed < SlowSpeed) ? SlowSpeed : speed;

			// calculate speed drop
			var drop = bleed * Time.Delta * frictionAmount;

			// apply speed drop
			float newspeed = speed - drop;
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

			// Check for floor against bounding box
			GetGroundEntity(TraceBBox(Position, Position + Vector3.Down * 2));

			bool onGround = GroundEntity != null;
			// Stop pawn from phasing through the floor with gravity
			if(onGround)
			{
				// Apply ground friction
				ApplyFriction();
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