using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    public class ActionController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;

        public ActionController(IInitiativeBusiness initiativeBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
        }

        // GET: Action
        public ActionResult _InitiativeInvitations()
        {
            var invitations = new List<InitiativeInvitationModel>();
            if (User.Identity.IsAuthenticated)
            {
                invitations = _initiativeBusiness.GetInvitations(User.Identity.Name).Select(x => new InitiativeInvitationModel
                {
                    InitiativeId = x.InitiativeId,
                    InitiativeName = x.InitiativeName,
                    InviteCode = x.InviteCode,
                }).ToList();
            }

            return View(new InitiativeInvitationsModel
            {
                Invitations = invitations
            });
        }
    }
}