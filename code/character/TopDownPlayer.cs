namespace Sandbox
{
	public partial class TopDownPlayer : Player
	{
		public TopDownPlayer(){

		}

		public override void Respawn()
		{

			base.Respawn();

			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new TopDownController();
			CameraMode = new TopDownCamera();
			Animator = new TopDownAnimator();
		}

		public override void Simulate( Client cl ) {

			base.Simulate( cl );

		}

		/// <summary>
		/// Called every frame on the client
		/// </summary>
		public override void FrameSimulate( Client cl ) {

			base.FrameSimulate( cl );

		}

	}
}