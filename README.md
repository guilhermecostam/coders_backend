# Coders
Este repositório se trata do back-end utilizado no projeto Coders.
<br>
<br>
Acessando o link abaixo será possível acessar o repositório do front-end e é nele também que está toda a documentação e demais informações sobre o projeto. 
<br>
[Repositório front-end do projeto](https://github.com/guilhermecostam/coders_frontend)

# Passo a passo de como rodar este projeto

1. Caso ainda não possua em sua máquina, faça a instalação do SDK [.NET 6.0](https://dotnet.microsoft.com/pt-br/download/dotnet/6.0). 
    1. Certifique-se de deixar a cli do dotnet disponível para seu uso ao menos na pasta do clone, é necessário ter o SDK instalado no mesmo disco em que está seu clone. 
    2. Clone este repositório e verifique se está tudo ok indo na pasta `./Coders-Back/` e executar um `dotnet build` e verificar a mensagem de sucesso.
    3. Caso tenha tido algum erro, execute os comandos `dotnet clear` e em seguida `dotnet restore` e tente novamente.
    4. Caso não funcione abra uma issue e nos marque, ou entre em contato por outros meios.
2. Rodar o SGBD SQL Server via docker (caso não queira usar docker, pode utilizar localmente, fique atento ao sub-passo *v*).
    1. Certifique-se de que possui o docker instalado e pronto para ser utilizado.
    2. Faça o download da imagem com `docker pull mcr.microsoft.com/mssql/server`.
    3. Execute o comando `docker volume create sqlserver_data` para criar um volume para persistir os dados e certifique-se de utilizar o mesmo nome `sqlserver_data` a frente da tag `-v` abaixo.
    4. Por fim, execute o comando `docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Pass123..' -p 1433:1433 --name sqlserver -v sqlserver_data:/var/opt/mssql -d mcr.microsoft.com/mssql/server` para subir o container.
    5. Fique atento ao comando acima, iremos utilizar essas informações para nos conectar ao banco de dados pela aplicação, se você mudar a porta, senha ou optar por utilizar outro usuário, certifique-se de mudar também a connection string no seguinte diretório (partindo do diretório deste arquivo) `./Coders-Back/Coders-Back.Host/appsettings.json`, atualize trocando os parâmetros `DefaultConnection": "Server=LocalAddress,Port;User ID=<YourUser>;Password=<YourPassword>;`.
3. Após ter o banco configurado e funcionando, devemos criar toda a estrutura dentro dele por meio do nosso ORM.
    1. Primeiro, rode o seguinte comando para baixar a CLI do EF Core `dotnet tool install --global dotnet-ef`.
    2. Após instalar a ferramenta, execute o seguinte comando na raíz do projeto: `dotnet ef database update --project ./Coders-Back/Coders-Back.Infrastructure --startup-project ./Coders-Back/Coders-Back.Host --verbose`.
4. Por fim, precisaremos de algum client para enviar requisições nossa API para podermos testar o nosso desenvolvimento, podemos usar então:
    1. Swagger (UI carregada automaticamente no navegador ao rodar o projeto).
    2. Insomnia (Open Source Client HTTP).
    3. Postman (Client HTTP).
    4. Fique a vontade para utilizar uma interface gráfica para utilizar melhor seu SGDB, utilize as mesmas informações do passo 3.*v* para se conectar atráves da ferramenta gráfica.
