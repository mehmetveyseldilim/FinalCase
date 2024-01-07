# Banking API Project


## Prerequisites

- Docker
- Docker Compose

## Installation

1. Clone the repository.
2. Start the containers:

## Commands

1) `git clone https://github.com/mehmetveyseldilim/FinalCase.git`

2) Go into the repository directory and via command line: `docker-compose up`

## Usage

Once the containers are up and running, you can access the application at `http://localhost:5154`
If you have `Postman` you can import the Postman json collection and start testing the app right away.

Also all operations have been executed and screenshotted. You can also access this documentation file when you clone this repository.


## Tests

1) `run dotnet test`

Unit tests should be run just okay. Integration tests should be passed except one. This issue is related quartz library and once quartz service registration line in `Program.cs` commented out, it will be executed too.





