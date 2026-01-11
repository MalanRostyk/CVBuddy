using CVBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVBuddy.Controllers
{
    public class ProjectController : HomeController
    {

        public ProjectController(UserManager<User> u, CVBuddyContext c, SignInManager<User> sm) : base(u, c, sm)
        {

        }

        //--------------------------------------------------READ PROJECTS--------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetProject()
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                var projects = await _context.Projects
                    .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.User)
                    .ToListAsync();
                if (projects == null)
                    throw new NullReferenceException("Projects could not be found.");

                var myProjects = new List<ProjectVM>();
                var otherProjects = new List<ProjectVM>();
                var publicProjects = new List<ProjectVM>();

                foreach (var project in projects)
                {
                    var usersInProject = project.ProjectUsers
                        .Select(pu => pu.User).ToList();

                    var activeUsers = usersInProject
                        .Where(u => !u.IsDeactivated).ToList();

                    var owner = project.ProjectUsers
                        .FirstOrDefault(pu => pu.IsOwner);

                    if (userId != null && project.ProjectUsers.Any(pu => pu.UserId == userId))
                    {
                        var relation = project.ProjectUsers
                            .FirstOrDefault(pu => pu.UserId == userId);

                        if (owner == null)
                            continue;

                        if (!relation!.IsOwner && owner.User.IsDeactivated)
                            continue;

                        myProjects.Add(new ProjectVM
                        {
                            Project = project,
                            UsersInProject = usersInProject,
                            ActiveUsers = activeUsers,
                            Relation = relation,
                            Owner = owner
                        });

                        continue;
                    }
                    if (userId != null)
                    {
                        if (owner == null || owner.User.IsDeactivated)
                            continue;

                        var isUserInProject = project.ProjectUsers
                            .Any(pu => pu.UserId == userId);

                        otherProjects.Add(new ProjectVM
                        {
                            Project = project,
                            UsersInProject = usersInProject,
                            ActiveUsers = activeUsers,
                            Owner = owner,
                            IsUserInProject = isUserInProject
                        });

                        continue;
                    }

                    if (owner == null || owner.User.IsDeactivated)
                        continue;

                    if (owner.User.HasPrivateProfile)
                        continue;

                    var publicUsers = activeUsers.Where(u => !u.HasPrivateProfile)
                        .ToList();

                    publicProjects.Add(new ProjectVM
                    {
                        Project = project,
                        UsersInProject = usersInProject,
                        ActiveUsers = publicUsers,
                        Owner = owner
                    });
                }
                var vm = new ProjectIndexViewModel
                {
                    MyProjects = myProjects,
                    OtherProjects = otherProjects,
                    PublicProjects = publicProjects
                };

                return View(vm);
            }
            catch (NullReferenceException ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = ex.Message });
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "An unexpected error occured while getting projects." });
            }
        }
        //--------------------------------------------------CREATE PROJECT--------------------------------------------------
        [HttpGet]
        public IActionResult CreateProject()
        {
            ViewBag.ProjectCreateHeadline = "Create a Project";

            ProjectViewModel projectVM = new();

            return View(projectVM);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectViewModel projectVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(projectVM);

                var userId = _userManager.GetUserId(User); //Hämtar användarens id
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    throw new NullReferenceException("User could not be found.");

                Project newProj = new Project
                {
                    Title = projectVM.Title,
                    Description = projectVM.Description,
                    StartDate = projectVM.StartDate ?? new DateTime(19000101),
                    Enddate = projectVM.Enddate,
                    PublishDate = projectVM.PublishDate
                };
                newProj.UsersInProject.Add(user);
                await _context.Projects.AddAsync(newProj);
                await _context.SaveChangesAsync();

                await _context.ProjectUsers.AddAsync(new ProjectUser //Lägg till ProjectUsers direkt i DbSet
                {
                    ProjId = newProj.Pid,
                    UserId = userId!,
                    IsOwner = true
                });
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error while creating your project." });
            }
            catch (NullReferenceException ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = ex.Message });
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "An unexpected error occured while creating a project." });
            }
        }
        //--------------------------------------------------UPDATE PROJECT--------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> UpdateProject(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                var project = await _context.Projects
                    .Include(p => p.ProjectUsers)
                    .FirstOrDefaultAsync(p => p.Pid == id && p.ProjectUsers.Any(pu => pu.UserId == userId));
                if (project == null)
                    throw new NullReferenceException("Project could not be found.");

                ProjectViewModel projectVM = new ProjectViewModel
                {
                    Pid = project.Pid,
                    Title = project.Title,
                    Description = project.Description,
                    StartDate = project.StartDate,
                    Enddate = project.Enddate,
                    UsersInProject = project.UsersInProject,
                    PublishDate = project.PublishDate
                };

                return View(projectVM);
            }
            catch (NullReferenceException ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = ex.Message });
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "An unexpected error occured while getting a project." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProject(ProjectViewModel projectToUpdate)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(projectToUpdate);

                var userId = _userManager.GetUserId(User);

                var project = await _context.Projects
                    .Include(pu => pu.ProjectUsers)
                    .FirstOrDefaultAsync(p => p.Pid == projectToUpdate.Pid && p.ProjectUsers.Any(pu => pu.UserId == userId));
                if (project == null)
                    throw new NullReferenceException("Project could not be found.");

                project.Title = projectToUpdate.Title;
                project.Description = projectToUpdate.Description;
                project.StartDate = projectToUpdate.StartDate ?? new DateTime(19000101);
                project.Enddate = projectToUpdate.Enddate;
                project.UsersInProject = projectToUpdate.UsersInProject;
                project.PublishDate = projectToUpdate.PublishDate;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error while updating your project." });
            }
            catch (NullReferenceException ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = ex.Message });
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "An unexpected error occured while updating a project." });
            }
        }
        //--------------------------------------------------JOIN PROJECT--------------------------------------------------
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProjectDetails(int PUId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    throw new NullReferenceException("User could not be found.");

                var projectuser = await _context.ProjectUsers
                    .Include(pu => pu.Project)
                    .FirstOrDefaultAsync(pu => pu.PUId == PUId);

                if (projectuser == null)
                    throw new NullReferenceException("Your relation to project could not be found.");

                bool alreadyJoined = await _context.ProjectUsers
                    .AnyAsync(pu => pu.ProjId == projectuser.ProjId && pu.UserId == userId);

                if (alreadyJoined)
                    return RedirectToAction("GetProject");

                await _context.ProjectUsers.AddAsync(new ProjectUser
                {
                    ProjId = projectuser.ProjId,
                    UserId = userId,
                    IsOwner = false
                });
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "There was an error while creating your relation to a project." });
            }
            catch (NullReferenceException ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = ex.Message });
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "An unexpected error occured while creating your relation to a project." });
            }
        }
    }
}
