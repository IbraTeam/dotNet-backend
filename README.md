* Сборка образа из директории с *.sln фалом: docker build -t dotnetbackend .
* Запуск контейнера, вместо порта 81 указать нужный: docker run -it -p 81:8080 dotnetbackend
* Перед сборкой изменить строку подключения (имя бд и пароль) для postgres (файл appsettings.json)
