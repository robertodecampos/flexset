using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Site.Models.UserViewModels;
using Site.Models;
using Site.Utils;
using Microsoft.AspNetCore.Authorization;
using Site.Exceptions;
using Site.Models._ViewModel;

namespace Site.Controllers
{
    public class UserController : Controller
    {

        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly Connection _connection;

        public UserController(
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            Connection connection
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _connection = connection;
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Index(Pagination pagination, OrderViewModel order, FiltroViewModel filtro)
        {
            ViewBag.CountPages = Math.Ceiling((decimal)UserModel.CountUsers(filtro, _connection, null) / pagination.PeerPage);
            if (ViewBag.CountPages == 0)
                ViewBag.CountPages = 1;
            if (pagination.Page > ViewBag.CountPages)
                pagination.Page = ViewBag.CountPages;
            ViewBag.Page = pagination.Page;
            ViewBag.PeerPage = pagination.PeerPage;
            ViewBag.Controller = "User";
            ViewBag.Action = nameof(Index);
            ViewBag.Filtro = filtro;
            ViewBag.Ordination = order;

            LoadFuncoes();

            IList<UserModel> usuarios = UserModel.GetUsers(
                filtro: filtro,
                fieldOrder: order.Campo,
                ordination: order.Ordenacao,
                limitInitial: (pagination.Page * pagination.PeerPage) - pagination.PeerPage,
                limitCount: pagination.PeerPage,
                conn: _connection,
                transaction: null
            );

            var usuariosRoles = new Dictionary<UserModel, string>(usuarios.Count);

            foreach (UserModel usuario in usuarios)
            {
                string roles = "";
                IList<string> lstRoles = UserRoleModel.GetNameRolesByIdUsuario(usuario.Id, _connection, null);

                for (int i = 0; i <= lstRoles.Count - 1; i++)
                {
                    roles += lstRoles[i];
                    if (i < (lstRoles.Count - 1))
                        roles += ", ";
                }

                usuariosRoles.Add(usuario, roles);
            }

            ViewBag.Usuarios = usuariosRoles;

            return View();
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            _signInManager.SignOutAsync();

            ViewData["returnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Cpf, model.Senha, model.Lembrar, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ViewBag.Mensagem = "CPF e/ou senha não encontrado!";
                return View(model);
            }

            using (var user = new UserModel())
            {
                if (!user.GetByNormalizedUserName(model.Cpf, _connection, null))
                {
                    ViewBag.Mensagem = "Ocorreu um problema ao tentar encontrar o usuário!";
                    return View(model);
                }

                if (user.PrimeiroAcesso)
                    return RedirectToAction("CreatePassword", new {
                        returnUrl = returnUrl,
                        senhaAtual = model.Senha
                    });
                else
                    return RedirectToLocal(returnUrl);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }

        private void LoadDataCreate() => LoadFuncoes();

        private void LoadFuncoes()
        {
            ViewBag.Funcoes = RoleModel.GetRoles(_connection, null);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            LoadDataCreate();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDataCreate();
                return View(model);
            }

            if (CreateUser(model))
                return RedirectToAction(nameof(Index));
            else
            {
                LoadDataCreate();
                return View(model);
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult CreateModal()
        {
            LoadDataCreate();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult CreateModal(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDataCreate();
                return View(model);
            }

            if (CreateUser(model))
                return Ok();
            else
            {
                LoadDataCreate();
                return View(model);
            }
        }

        [Authorize]
        public IActionResult CreatePassword(string senhaAtual, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.SenhaAtual = senhaAtual;

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePassword(CreatePasswordViewModel model, string senhaAtual, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Mensagem = "Algumas informações não parecem estar consistentes!";
                ViewBag.ReturUrl = returnUrl;
                return View(model);
            }

            using (UserModel user = await _userManager.GetUserAsync(User))
            {
                IdentityResult result = await _userManager.ChangePasswordAsync(user, senhaAtual, model.Senha);

                if (!result.Succeeded)
                {
                    ViewBag.Mensagem = "Não foi possível cadastrar sua senha!";
                    ViewBag.ReturUrl = returnUrl;
                    return View(model);
                }
                
                try
                {
                    user.PrimeiroAcesso = false;
                    user.Salvar(_connection, null);
                    return RedirectToLocal(returnUrl);
                } catch
                {
                    ViewBag.Mensagem = "Ocorreu um erro ao cadastrar sua senha!";
                    ViewBag.ReturUrl = returnUrl;
                    return View(model);
                }
            }
        }

        [Authorize]
        public IActionResult AlterLogged()
        {
            using (var user = new UserModel())
            {
                if (!user.GetById(int.Parse(_userManager.GetUserId(User)), _connection, null))
                    throw new SiteException("Não foi possível encontrar o usuário!");

                ViewBag.User = user;
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult AlterPersonalInformationsLogged(PersonalInformationsViewModal model)
        {
            if (!ModelState.IsValid)
            {
                TempData["MensagemAlterPersonalInformations"] = "As informações não estão consistentes!";
                return View(model);
            }

            try
            {
                if (!SavePersonalInformations(model, int.Parse(_userManager.GetUserId(User))))
                {
                    TempData["MensagemAlterPersonalInformations"] = "Não foi possível encontrar o usuário logado!";
                    return View(model);
                }

                return RedirectToAction("AlterInformation");
            }
            catch (Exception e)
            {
                TempData["MensagemAlterPersonalInformations"] = "Ocorreu um erro inesperado!";
                return View(model);
            }

        }

        public IActionResult AlterModal(int id)
        {
            if (id == 0)
                return BadRequest(new {
                    Mensagem = "O id do usuário não foi informado!"
                });

            using (var user = new UserModel())
            {
                if (!user.GetById(id, _connection, null))
                    return BadRequest(new
                    {
                        Mensagem = $"Não foi encontrado nenhum usuário com o id '{id}'!"
                    });

                LoadFuncoes();

                return View(new AlterViewModel() {
                    Id = user.Id,
                    Nome = user.Nome,
                    Email = user.Email,
                    DataNascimento = user.DataNascimento,
                    Funcoes = UserRoleModel.GetNormalizedNameRolesByIdUsuario(user.Id, _connection, null)
                });
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult AlterModal(AlterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadFuncoes();
                return View(model);
            }

            using (var user = new UserModel())
            {
                if (!user.GetById(model.Id, _connection, null))
                {
                    LoadFuncoes();
                    return View(model);
                }

                user.Nome = model.Nome;
                user.Email = model.Email;
                user.DataNascimento = (DateTime)model.DataNascimento;

                user.Salvar(_connection, null);

                // Remover funções
                foreach (var funcao in UserRoleModel.GetNormalizedNameRolesByIdUsuario(user.Id, _connection, null))
                {
                    if (model.Funcoes.Where(a => a.Equals(funcao)).Count() == 0)
                    {
                        using (var role = new RoleModel())
                        {
                            if (!role.GetByNomeNormalized(funcao, _connection, null))
                                continue;

                            using (var userRole = new UserRoleModel())
                            {
                                if (!userRole.GetByIdUsuarioAndIdRole(user.Id, role.Id, _connection, null))
                                    continue;

                                userRole.Remover(_connection, null);
                            }
                        }
                    }
                }

                // Adicionar funções
                foreach (var funcao in model.Funcoes)
                {
                    if (!UserRoleModel.UserInRole(user.Id, funcao, _connection, null))
                    {
                        Task<IdentityResult> result = _userManager.AddToRoleAsync(user, funcao);
                        result.Wait();
                    }
                }

                return Ok();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AlterPassword(AlterPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Mensagem = "Algumas informações não parecem estar consistentes!";
                return View(model);
            }

            using (UserModel user = await _userManager.GetUserAsync(User))
            {
                IdentityResult result = await _userManager.ChangePasswordAsync(user, model.SenhaAtual, model.Senha);

                if (!result.Succeeded)
                {
                    ViewBag.Mensagem = "Não foi possível alterar sua senha!";
                    return View(model);
                }

                try
                {
                    user.PrimeiroAcesso = false;
                    user.Salvar(_connection, null);
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                catch
                {
                    ViewBag.Mensagem = "Ocorreu um erro ao alterar sua senha!";
                    return View(model);
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Remove(IList<int> users)
        {
            if ((users == null) || ((users != null) && users.Count == 0))
                return BadRequest(new {
                    Mensagem = "Nenhum usuário passado!"
                });

            int qtdeRemovido = 0;

            foreach (int idUser in users)
            {
                using (var user = new UserModel())
                {
                    if (!user.GetById(idUser, _connection, null))
                        continue;

                    try
                    {
                        user.Remove(_connection, null);
                        qtdeRemovido += 1;
                    }
                    finally { }
                }
            }

            return Ok(new {
                Removidos = qtdeRemovido
            });
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private bool SavePersonalInformations(PersonalInformationsViewModal model, int? id)
        {
            using (var user = new UserModel())
            {
                if (!user.GetById((int)id, _connection, null))
                    return false;

                user.Nome = model.Nome;
                user.Email = model.Email;
                user.DataNascimento = (DateTime)model.DataNascimento;

                user.Salvar(_connection, null);

                return true;
            }
        }

        private bool CreateUser(CreateViewModel model)
        {
            var user = new UserModel()
            {
                Nome = model.Nome,
                Cpf = model.Cpf,
                Email = model.Email,
                UserName = model.Cpf,
                DataNascimento = (DateTime)model.DataNascimento,
                PrimeiroAcesso = true
            };

            Task<IdentityResult> result = _userManager.CreateAsync(user, "Flex!" + user.Cpf);
            result.Wait();
            IdentityResult identityResult = result.Result;

            if (identityResult.Succeeded)
            {
                foreach (string funcao in model.Funcoes)
                {
                    result = _userManager.AddToRoleAsync(user, funcao);
                    result.Wait();
                }

                return true;
            }
            else
            {
                var errosDuplicate = identityResult.Errors.Where(erro => erro.Code == "DuplicateUserName").ToList();

                if (errosDuplicate.Count > 0)
                    ViewData["erro"] = "Este CPF já está cadastrado no sistema!";
                else
                    ViewData["erro"] = identityResult.Errors.ToString();

                return false;
            }
        }
    }
}