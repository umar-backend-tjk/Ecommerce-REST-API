namespace Infrastructure.Constants;

public static class HtmlPages
{
  public const string WelcomeMail = """
                                    <!DOCTYPE html>
                                    <html lang="ru" style="font-family: Arial, sans-serif;">
                                    <body style="margin:0; padding:0; background-color:#f7f7f7;">
                                      <table align="center" width="100%" cellpadding="0" cellspacing="0" style="max-width:600px; background:white; border-radius:8px; padding:30px;">
                                        <tr>
                                          <td style="text-align:center;">
                                            <h2 style="color:#333;">Добро пожаловать в <b>E-Commerce</b>! 🎉</h2>
                                          </td>
                                        </tr>

                                        <tr>
                                          <td style="padding-top:10px; font-size:16px; color:#444;">
                                            <p>Здравствуйте, пользователь!</p>

                                            <p>
                                              Спасибо, что зарегистрировались в нашем интернет-магазине.  
                                              Теперь для вас доступны:
                                            </p>

                                            <ul style="color:#444;">
                                              <li>удобное и быстрое оформление заказов</li>
                                              <li>отслеживание статуса покупок</li>
                                              <li>личный список желаний</li>
                                              <li>персональные рекомендации</li>
                                              <li>доступ к эксклюзивным акциям и промокодам</li>
                                            </ul>

                                            <p>
                                              Мы рады, что вы с нами, и постараемся сделать ваш опыт покупок максимально приятным!
                                            </p>

                                            <p style="margin-top:30px; font-weight:bold; color:#333;">
                                              Приятных покупок! 🛍  
                                              <br>Команда <b>E-Commerce</b>
                                            </p>
                                          </td>
                                        </tr>

                                        <tr>
                                          <td style="font-size:12px; color:#888; text-align:center; padding-top:20px;">
                                            Это письмо отправлено автоматически. Пожалуйста, не отвечайте на него.
                                          </td>
                                        </tr>
                                      </table>
                                    </body>
                                    </html>
                                    """;
}