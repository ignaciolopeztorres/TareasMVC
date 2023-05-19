﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Abstractions;
using System.Security.Claims;
using TareasMVC.Models;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext context;

        public UsuariosController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Registro()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = new IdentityUser() { Email = model.Email, UserName = model.Email };

            var resultado = await userManager.CreateAsync(usuario, password: model.Password);

            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        [AllowAnonymous]
        public IActionResult Login(string mensaje = null)
        {
            if (mensaje is not null)
            {
                ViewData["mensaje"] = mensaje;
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var resultado = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult LoginExterno(string proveedor, string UrlRetorno = null)
        {
            var urlRedireccion = Url.Action("RegistrarUsuarioExterno", values: new { UrlRetorno });
            var propiedades = signInManager.ConfigureExternalAuthenticationProperties(proveedor, urlRedireccion);
            return new ChallengeResult(proveedor, propiedades);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuarioExterno(string urlRetorno = null,
            string remoteError = null)
        {
            urlRetorno = urlRetorno ?? Url.Content("~/");

            var mensajes = "";

            if (remoteError is not null)
            {
                mensajes = $"Error del proovedor externo{remoteError}";
                return RedirectToAction("login", routeValues: new { mensajes });
            }

            var info = await signInManager.GetExternalLoginInfoAsync();

            if (info is null)
            {
                mensajes = "Error Cargando la data de login externo";
                return RedirectToAction("login", routeValues: new { mensajes });
            }

            var resultadoLoginExterno = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (resultadoLoginExterno.Succeeded)
            {
                return LocalRedirect(urlRetorno);
            }
            string email = "";

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                mensajes = "Error leyendo el emails del usuario del proveedor";
                return RedirectToAction("login", routeValues: new { mensajes });
            }

            var usuario = new IdentityUser { Email = email, UserName = email };

            var resultadoCrearUSuario = await userManager.CreateAsync(usuario);

            if (!resultadoCrearUSuario.Succeeded)
            {
                mensajes = resultadoCrearUSuario.Errors.First().Description;
                return RedirectToAction("login", routeValues: new { mensajes });
            }

            var resultadoAgregarLogin = await userManager.AddLoginAsync(usuario, info);

            if (resultadoAgregarLogin.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true, info.LoginProvider);
                return LocalRedirect(urlRetorno);
            }

            mensajes = "A ocurrido un error a agregar el login";
            return RedirectToAction("login", routeValues: new { mensajes });
        }

        [HttpGet]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> Listado(string mensaje = null)
        {
            var usuarios = await context.Users.Select(u => new UsuarioViewModel { Email = u.Email }).ToListAsync();

            var model = new UsuariosListadoViewModel();
            model.Usuarios = usuarios;
            model.Mensaje = mensaje;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> HacerAdmin(string email)
        {
            var usuario = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (usuario is null)
            {
                return NotFound();
            }
            await userManager.AddToRoleAsync(usuario, Constantes.RolAdmin);

            return RedirectToAction("Listado",
                routeValues: new { mensaje = "Rol asignado correctamente a " + email });
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> RemoverAdmin(string email)
        {
            var usuario = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (usuario is null)
            {
                return NotFound();
            }
            await userManager.RemoveFromRoleAsync(usuario, Constantes.RolAdmin);

            return RedirectToAction("Listado",
                routeValues: new { mensaje = "Rol removido correctamente a " + email });
        }
    }
}