using Microsoft.AspNetCore.Mvc;

namespace Server.Filters;

public class JwtAuthAttribute() : TypeFilterAttribute(typeof(JwtAuthFilter));