namespace Syspharma.API.Services
{
	public static class EmailTemplates
	{
		// =====================================================
		// BIENVENIDA — Cliente se registra solo
		// =====================================================
		public static string Bienvenida(string nombre, string email, string frontendUrl) => $@"
<!DOCTYPE html>
<html lang='es'>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'></head>
<body style='margin:0;padding:0;background-color:#f4f7f6;font-family:Arial,Helvetica,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f7f6;padding:40px 0;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0' style='background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);'>
          
          <!-- HEADER -->
          <tr>
            <td style='background-color:#059669;padding:32px 40px;text-align:center;'>
              <table cellpadding='0' cellspacing='0' style='margin:0 auto 16px auto;'>
                <tr>
                  <td style='background-color:rgba(255,255,255,0.2);border-radius:12px;padding:12px 20px;'>
                    <span style='color:#ffffff;font-size:22px;font-weight:900;letter-spacing:1px;'>💊 SysPharma</span>
                  </td>
                </tr>
              </table>
              <h1 style='color:#ffffff;margin:0;font-size:26px;font-weight:700;'>¡Bienvenido a SysPharma!</h1>
              <p style='color:rgba(255,255,255,0.85);margin:8px 0 0 0;font-size:14px;'>Tu cuenta ha sido creada exitosamente</p>
            </td>
          </tr>

          <!-- BODY -->
          <tr>
            <td style='padding:40px;'>
              <p style='color:#374151;font-size:16px;margin:0 0 16px 0;'>Hola, <strong>{nombre}</strong> 👋</p>
              <p style='color:#6b7280;font-size:14px;line-height:1.6;margin:0 0 24px 0;'>
                Nos alegra tenerte con nosotros. Tu cuenta ha sido registrada correctamente y ya puedes acceder a todos los beneficios de SysPharma.
              </p>

              <!-- INFO BOX -->
              <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f0fdf4;border:1px solid #bbf7d0;border-radius:8px;margin-bottom:24px;'>
                <tr>
                  <td style='padding:20px 24px;'>
                    <p style='color:#065f46;font-size:13px;font-weight:700;margin:0 0 12px 0;text-transform:uppercase;letter-spacing:0.5px;'>Datos de tu cuenta</p>
                    <table cellpadding='0' cellspacing='0'>
                      <tr>
                        <td style='color:#6b7280;font-size:13px;padding:4px 0;width:80px;'>Nombre:</td>
                        <td style='color:#111827;font-size:13px;font-weight:600;padding:4px 0;'>{nombre}</td>
                      </tr>
                      <tr>
                        <td style='color:#6b7280;font-size:13px;padding:4px 0;'>Correo:</td>
                        <td style='color:#111827;font-size:13px;font-weight:600;padding:4px 0;'>{email}</td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

              <!-- BENEFICIOS -->
              <p style='color:#374151;font-size:14px;font-weight:700;margin:0 0 12px 0;'>¿Qué podés hacer ahora?</p>
              <table width='100%' cellpadding='0' cellspacing='0' style='margin-bottom:28px;'>
                <tr>
                  <td style='padding:8px 0;'>
                    <table cellpadding='0' cellspacing='0'>
                      <tr>
                        <td style='background-color:#dcfce7;border-radius:50%;width:28px;height:28px;text-align:center;vertical-align:middle;font-size:14px;'>🛍️</td>
                        <td style='padding-left:12px;color:#374151;font-size:13px;'>Explorar el catálogo de productos</td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style='padding:8px 0;'>
                    <table cellpadding='0' cellspacing='0'>
                      <tr>
                        <td style='background-color:#dcfce7;border-radius:50%;width:28px;height:28px;text-align:center;vertical-align:middle;font-size:14px;'>📦</td>
                        <td style='padding-left:12px;color:#374151;font-size:13px;'>Realizar y hacer seguimiento de pedidos</td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style='padding:8px 0;'>
                    <table cellpadding='0' cellspacing='0'>
                      <tr>
                        <td style='background-color:#dcfce7;border-radius:50%;width:28px;height:28px;text-align:center;vertical-align:middle;font-size:14px;'>🩺</td>
                        <td style='padding-left:12px;color:#374151;font-size:13px;'>Agendar citas médicas</td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

              <!-- CTA BUTTON -->
              <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>
                  <td align='center'>
                    <a href='{frontendUrl}/login' 
                       style='display:inline-block;background-color:#059669;color:#ffffff;text-decoration:none;padding:14px 40px;border-radius:8px;font-size:14px;font-weight:700;letter-spacing:0.5px;'>
                      Ingresar a mi cuenta →
                    </a>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- FOOTER -->
          <tr>
            <td style='background-color:#f9fafb;border-top:1px solid #e5e7eb;padding:24px 40px;text-align:center;'>
              <p style='color:#9ca3af;font-size:12px;margin:0 0 4px 0;'>Si no creaste esta cuenta, podés ignorar este correo.</p>
              <p style='color:#9ca3af;font-size:12px;margin:0;'>© {DateTime.Now.Year} SysPharma. Todos los derechos reservados.</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";

		// =====================================================
		// BIENVENIDA — Admin crea usuario (con contraseña temporal)
		// =====================================================
		public static string BienvenidaConCredenciales(string nombre, string email, string passwordTemporal, string frontendUrl) => $@"
<!DOCTYPE html>
<html lang='es'>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'></head>
<body style='margin:0;padding:0;background-color:#f4f7f6;font-family:Arial,Helvetica,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f7f6;padding:40px 0;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0' style='background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);'>

          <!-- HEADER -->
          <tr>
            <td style='background-color:#059669;padding:32px 40px;text-align:center;'>
              <table cellpadding='0' cellspacing='0' style='margin:0 auto 16px auto;'>
                <tr>
                  <td style='background-color:rgba(255,255,255,0.2);border-radius:12px;padding:12px 20px;'>
                    <span style='color:#ffffff;font-size:22px;font-weight:900;letter-spacing:1px;'>💊 SysPharma</span>
                  </td>
                </tr>
              </table>
              <h1 style='color:#ffffff;margin:0;font-size:26px;font-weight:700;'>¡Tu cuenta está lista!</h1>
              <p style='color:rgba(255,255,255,0.85);margin:8px 0 0 0;font-size:14px;'>El administrador ha creado tu acceso a SysPharma</p>
            </td>
          </tr>

          <!-- BODY -->
          <tr>
            <td style='padding:40px;'>
              <p style='color:#374151;font-size:16px;margin:0 0 16px 0;'>Hola, <strong>{nombre}</strong> 👋</p>
              <p style='color:#6b7280;font-size:14px;line-height:1.6;margin:0 0 24px 0;'>
                Se ha creado una cuenta para vos en SysPharma. A continuación encontrás tus credenciales de acceso.
              </p>

              <!-- CREDENCIALES BOX -->
              <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f0fdf4;border:1px solid #bbf7d0;border-radius:8px;margin-bottom:24px;'>
                <tr>
                  <td style='padding:20px 24px;'>
                    <p style='color:#065f46;font-size:13px;font-weight:700;margin:0 0 12px 0;text-transform:uppercase;letter-spacing:0.5px;'>Tus credenciales de acceso</p>
                    <table cellpadding='0' cellspacing='0'>
                      <tr>
                        <td style='color:#6b7280;font-size:13px;padding:6px 0;width:110px;'>Correo:</td>
                        <td style='color:#111827;font-size:13px;font-weight:600;padding:6px 0;'>{email}</td>
                      </tr>
                      <tr>
                        <td style='color:#6b7280;font-size:13px;padding:6px 0;'>Contraseña:</td>
                        <td style='padding:6px 0;'>
                          <span style='background-color:#dcfce7;color:#065f46;font-size:13px;font-weight:700;padding:4px 10px;border-radius:4px;font-family:monospace;letter-spacing:1px;'>{passwordTemporal}</span>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

              <!-- WARNING -->
              <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#fffbeb;border:1px solid #fde68a;border-radius:8px;margin-bottom:28px;'>
                <tr>
                  <td style='padding:14px 20px;'>
                    <p style='color:#92400e;font-size:13px;margin:0;'>⚠️ <strong>Importante:</strong> Te recomendamos cambiar tu contraseña después de ingresar por primera vez desde tu perfil.</p>
                  </td>
                </tr>
              </table>

              <!-- CTA BUTTON -->
              <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>
                  <td align='center'>
                    <a href='{frontendUrl}/login'
                       style='display:inline-block;background-color:#059669;color:#ffffff;text-decoration:none;padding:14px 40px;border-radius:8px;font-size:14px;font-weight:700;letter-spacing:0.5px;'>
                      Ingresar a mi cuenta →
                    </a>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- FOOTER -->
          <tr>
            <td style='background-color:#f9fafb;border-top:1px solid #e5e7eb;padding:24px 40px;text-align:center;'>
              <p style='color:#9ca3af;font-size:12px;margin:0 0 4px 0;'>Si recibiste este correo por error, podés ignorarlo.</p>
              <p style='color:#9ca3af;font-size:12px;margin:0;'>© {DateTime.Now.Year} SysPharma. Todos los derechos reservados.</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";

		// =====================================================
		// RECUPERACIÓN DE CONTRASEÑA — Mejorado
		// =====================================================
		public static string RecuperacionContrasena(string nombre, string codigo) => $@"
<!DOCTYPE html>
<html lang='es'>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'></head>
<body style='margin:0;padding:0;background-color:#f4f7f6;font-family:Arial,Helvetica,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f7f6;padding:40px 0;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0' style='background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);'>

          <!-- HEADER -->
          <tr>
            <td style='background-color:#059669;padding:32px 40px;text-align:center;'>
              <table cellpadding='0' cellspacing='0' style='margin:0 auto 16px auto;'>
                <tr>
                  <td style='background-color:rgba(255,255,255,0.2);border-radius:12px;padding:12px 20px;'>
                    <span style='color:#ffffff;font-size:22px;font-weight:900;letter-spacing:1px;'>💊 SysPharma</span>
                  </td>
                </tr>
              </table>
              <h1 style='color:#ffffff;margin:0;font-size:26px;font-weight:700;'>Recuperación de Contraseña</h1>
              <p style='color:rgba(255,255,255,0.85);margin:8px 0 0 0;font-size:14px;'>Solicitaste restablecer tu contraseña</p>
            </td>
          </tr>

          <!-- BODY -->
          <tr>
            <td style='padding:40px;'>
              <p style='color:#374151;font-size:16px;margin:0 0 16px 0;'>Hola, <strong>{nombre}</strong> 👋</p>
              <p style='color:#6b7280;font-size:14px;line-height:1.6;margin:0 0 28px 0;'>
                Recibimos una solicitud para restablecer la contraseña de tu cuenta. Usá el siguiente código para continuar:
              </p>

              <!-- CÓDIGO -->
              <table width='100%' cellpadding='0' cellspacing='0' style='margin-bottom:28px;'>
                <tr>
                  <td align='center'>
                    <div style='background-color:#f0fdf4;border:2px dashed #059669;border-radius:12px;padding:24px 40px;display:inline-block;'>
                      <p style='color:#6b7280;font-size:12px;margin:0 0 8px 0;text-transform:uppercase;letter-spacing:1px;font-weight:700;'>Tu código de verificación</p>
                      <p style='color:#059669;font-size:36px;font-weight:900;margin:0;letter-spacing:10px;font-family:monospace;'>{codigo}</p>
                    </div>
                  </td>
                </tr>
              </table>

              <!-- WARNING -->
              <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#fffbeb;border:1px solid #fde68a;border-radius:8px;margin-bottom:24px;'>
                <tr>
                  <td style='padding:14px 20px;'>
                    <p style='color:#92400e;font-size:13px;margin:0;'>⏱️ Este código es válido por <strong>15 minutos</strong>. No lo compartas con nadie.</p>
                  </td>
                </tr>
              </table>

              <p style='color:#9ca3af;font-size:13px;margin:0;'>Si no solicitaste este cambio, podés ignorar este correo. Tu contraseña no será modificada.</p>
            </td>
          </tr>

          <!-- FOOTER -->
          <tr>
            <td style='background-color:#f9fafb;border-top:1px solid #e5e7eb;padding:24px 40px;text-align:center;'>
              <p style='color:#9ca3af;font-size:12px;margin:0;'>© {DateTime.Now.Year} SysPharma. Todos los derechos reservados.</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
	}
}