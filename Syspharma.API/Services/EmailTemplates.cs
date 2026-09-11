namespace Syspharma.API.Services
{
	public static class EmailTemplates
	{
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
