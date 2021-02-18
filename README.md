# MessageBrokerServiceApi
Desenvolvimento de um microserviço rodando em docker disparando mensagens através do broker RabbitMQ. Projeto utilizando a implementação de Fanout.

Sugestão de Execução Inicial:

1) Executar o docker-compose para realizar o download da imagem e levantar a aplicação em container.
2) Executar o docker-compose do RabbitMq.
3) Acessar o swagger do projeto e realizar a chamada do endpoint "start". Feito isso será realizado um disparo de 5 em 5 segundos para as filas do RabbitMq.
4) Consultar periodicamente o endpoint "messages" para visualizar as mensagens recebidas pelo serviço (disparada para ele mesmo).
5) Para obter informações do id do serviço, basta executar o endpoint "info".




Sugestão de Execução Multi Processamento:

1) Após os passos anteriores, abrir o terminal e executar o comando "docker run -d -p 55701:80 --network=rag messagebrokerserviceapi".
2) Após a execução da primeira instância, rode o comando "docker run -d -p 55702:80 --network=rag messagebrokerserviceapi".
3) Acesse ambos containers pelo postman ou utilizando a ferramenta que preferir.
4) Consultar periodicamente o endpoint "messages" de ambos os endpoints para visualizar a troca de mensagens.





Considerações:

1) A estratégia de comunicação utilizada foi através do uso da exchange de Fanout. Logo, as mensagens são disparadas para todas as filas que fizeram bind na exchange.
2) Para uso de melhores práticas é fortemente recomendado que os helpers e recursos comuns a todas aplicações sejam apartadas do código em um package nugget.
