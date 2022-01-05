using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FondoEstimulo.Models.Aplicacion
{
    public class Usuario : IdentityUser
    {
        #region Public Properties

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }

        #endregion Public Properties
    }
}