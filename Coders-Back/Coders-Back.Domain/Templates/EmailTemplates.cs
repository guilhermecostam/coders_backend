namespace Coders_Back.Domain.Templates;

public static class EmailTemplates
{
  private const string Quote = "\"";

  public static string ConfirmationTemplate(string confirmationUrl) => @$"<!DOCTYPE html>
<html lang={Quote}en{Quote}>
<head>
  <meta charset={Quote}UTF-8{Quote}>
  <meta name={Quote}viewport{Quote} content={Quote}width=device-width, initial-scale=1.0{Quote}>
</head>
<body 
  style={Quote}
    background-color: #fff; 
    margin: 0; 
    padding: 0{Quote}
>
  <div 
    style={Quote}
      max-width: 600px; 
      height: 100vh; 
      background-image: 
      linear-gradient(#620C9B, #43056C);
      margin: 0 auto;
  {Quote}>
    <table 
      cellspacing={Quote}0{Quote}
      cellpadding={Quote}0{Quote}
      style={Quote}
        color: #FBFBFB;
        font-family: 'Montserrat', 'Helvetica', 'verdana', sans-serif;
        width: 100%;
        text-align: center;
        max-width: 600px;
        padding: 200px 20px 40px 20px;
      {Quote}
    >
      <tr>
        <th>
          <h1 
            style={Quote}
              font-size: 48px; 
              font-weight: 900; 
              margin: 0;{Quote}>CODERS</h1>
        </th>
      </tr>
      <tr>
        <td>Para confirmar o seu e-mail basta clicar no link abaixo</td>
      </tr>
      <tr>
        <td>
          <a 
            href={Quote}{confirmationUrl}{Quote}
            target={Quote}_blank{Quote}
            style={Quote}
              display: block;
              max-width: fit-content;
              border: 2px solid #FBFBFB;
              color: #FBFBFB;
              font-size: 16px;
              font-weight: 700;
              text-decoration: none;
              border-radius: 5px;
              margin: 40px auto 40px auto;
              padding: 10px 20px;
              {Quote}
          >CONFIRMAR CONTA</a>
        </td>
      </tr>
      <tr>
        <td>Copyright © 2023 | Coders</td>
      </tr>
    </table>
  </div>
</body>
</html>";
  
  public static string ConfirmedTemplate() => @$"<!DOCTYPE html>
<html lang={Quote}en{Quote}>
<head>
  <meta charset={Quote}UTF-8{Quote}>
  <meta name={Quote}viewport{Quote} content={Quote}width=device-width, initial-scale=1.0{Quote}>
</head>
<body 
  style={Quote}
    background-color: #fff; 
    margin: 0; 
    padding: 0{Quote}
>
  <div 
    style={Quote}
      max-width: 600px; 
      height: 100vh; 
      background-image: 
      linear-gradient(#620C9B, #43056C);
      margin: 0 auto;
  {Quote}>
    <table 
      cellspacing={Quote}0{Quote}
      cellpadding={Quote}0{Quote}
      style={Quote}
        color: #FBFBFB;
        font-family: 'Montserrat', 'Helvetica', 'verdana', sans-serif;
        width: 100%;
        text-align: center;
        max-width: 600px;
        padding: 200px 20px 40px 20px;
      {Quote}
    >
      <tr>
        <th>
          <h1 
            style={Quote}
              font-size: 48px; 
              font-weight: 900; 
              margin: 0;{Quote}>CODERS</h1>
        </th>
      </tr>
      <tr>
        <td>O seu e-mail foi confirmado com sucesso</td>
      </tr>
      <tr>
        <td>Acesse a página de login e entre com sua conta</td>
      </tr>
      <tr>
        <td>Copyright © 2023 | Coders</td>
      </tr>
    </table>
  </div>
</body>
</html>";

}
