using InsideModel.Helper;
using Microsoft.AspNet.Identity;

namespace InsideModel.Models.Identity
{
    public class InsideUserManager : UserManager<InsideUser>
    {
       
        public InsideUserManager(IUserStore<InsideUser> userStore ) : base(userStore)
        {
            this.PasswordHasher = new SQLPasswordHasher();
            this.MaxFailedAccessAttemptsBeforeLockout = 5;
            this.UserValidator = new UserValidator<InsideUser>(this) { AllowOnlyAlphanumericUserNames = false };
        }

        protected override void Dispose(bool disposing)
        {
            var t = disposing;
            base.Dispose(disposing);
        }
    }

    public class InsideRoleManager : RoleManager<InsideRole>
    {

        public InsideRoleManager(IRoleStore<InsideRole> roleStore): base(roleStore){}

        protected override void Dispose(bool disposing)
        {
            var t = disposing;
            base.Dispose(disposing);
        }

    }
}
