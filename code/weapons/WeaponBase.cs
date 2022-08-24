namespace Sandbox
{
    public abstract partial class WeaponBase
    {

        public PawnController pawnController;

        public WeaponBase(PawnController _pawnController)
        {
            pawnController = _pawnController;
        }

        public virtual void Fire()
        {
            
        }
    }
}