# SEEK Pact Based Service Simulator

SEEK.Automation.Phantom is a restfull service simulator. Once it is launched you can use it to run in-memory services listening on different ports, and serving responses to the incoming requests. Each simulated service can be configured to respond with a single response or based on the specified interactions in a pact. This is a similar concept to [the stubbing library](https://github.com/SEEK-Jobs/seek.automation.stub).

Please also note all simulations registrations can be done while the service is running and no restart is required.
You also will have the ability to load the last simulaitons from cache.

## Build

To build the solution:

```> .\build.ps1 -target Build-Solution```

To run the tests:

```> .\build.ps1 -target Run-Unit-Tests```

## Installation

To install this service run the following command:

```
SEEK.Automation.Phantom.exe install
```

## Firewall Rules

In order for Phantom to launch simulations on different ports, your firewall needs to be configured to allow inbound connections to these ports. Phantom has the ability to create
the rules for your firewall. However, this feature is disabled by default. This means you have two options. You can create the rules manually or enable the feature in Phantom. 
However, please assess the security risks for your environment before doing so.

In the app.config of the Phantom, you will see the following options:

```
<add key="SEEK.Automation.Phantom.Firewall.Create.Rules" value="False"/>
<add key="SEEK.Automation.Phantom.Firewall.Port.Range.From" value="9000"/>
<add key="SEEK.Automation.Phantom.Firewall.Port.Range.To" value="9025"/>
```

The above allows you to enable the rule creation as well as the ability to specify the port range.

## Default Values

Here are the default values for the service:

* Phantom's default URL: http://localhost:8080
* SEEK.Automation.Phantom.Firewall.Create.Rules is set to false. This prevents Phantom to open firewall ports.
* Default firewall port ranges: 9000~9025

## Configuration 

Once the Phantom service is running you can use Phanom's endpoints to register for simulated services. There are different registeration types. The registration types are currently:

* register  this allows to serve a single response to any request
* pact      this allows to specify a pact and serve responses based on the interactions in the pact
* redirect  this allows to redirect a request to another server
* log       this allows to log the last incoming request to a file

Please see below for examples on how to simulate and configure services.

## Example 1: Phantom's Health and Available Ports

Phantom has couple of handy endpoints. For example to check the health, do the following:

```
GET http://localhost:8080/v1/health
```

This should return Ok.

Now if you would like to see the list of used ports on the machine where Phantom is running do the following:

```
http://localhost:8080/v1/list
```

This will return a list similar to what you see below:

```
Port#     Process Name                                                Port Name
80        System                                                      System (TCPv4 port 80)
135       svchost                                                     svchost (TCPv4 port 135)
445       System                                                      System (TCPv4 port 445)
2107      mqsvc                                                       mqsvc (TCPv4 port 2107)
5357      System                                                      System (TCPv4 port 5357)
8080      System                                                      System (TCPv4 port 8080)
9003      System                                                      System (TCPv4 port 9003)
49152     System                                                      System (TCPv4 port 49152)
```

This will give you a better view on what ports are available for use. Please don't forget to configure your firewall as discussed previously.

## Example 2: Single Response Simulation

If you want to simulate a service where every request is treated the same by sending back the same response, you can
use the ```register``` registeration type. So for example to request from Phantom to run a simulated service on 
port 9000 do the following:

```
POST http://localhost:8080/v1/simulate?type=register&port=9000
```

with payload:

```json
{
	  "id": 1,
	  "username": "John""
}
```

Now any request that comes in on port 9000 will recieve the following response:

```json
{
	  "id": 1,
	  "username": "John""
}
```

## Example 3: Pact Based Simulation

If you have a pact already, you can use that as the basis for performing simulations. For example if you have the following pact:

```json
{
  "provider": {
    "name": "Dad"
  },
  "consumer": {
    "name": "Child"
  },
  "interactions": [
    {
      "description": "a request for money",
      "provider_state": "Dad has enough money",
      "request": {
        "method": "post",
        "path": "/please/give/me/some/money",
        "headers": {
          "Content-Type": "application/json; charset=utf-8"
        }
      },
      "response": {
        "status": 200
      }
    }
  ]
}
```

Then you can create a simulated service by using the ```pact``` registration type. So to simualte on ```port 9001```:

```
POST http://localhost:8080/v1/simulate?type=pact&port=9001
```

With payload:

```json
{
  "provider": {
    "name": "Dad"
  },
  "consumer": {
    "name": "Child"
  },
  "interactions": [
    {
      "description": "a request for money",
      "provider_state": "Dad has enough money",
      "request": {
        "method": "post",
        "path": "/please/give/me/some/money",
        "headers": {
          "Content-Type": "application/json; charset=utf-8"
        }
      },
      "response": {
        "status": 200
      }
    }
  ]
}
```
Now after you have done the above registration, if you do the following post:

```
POST http://localhost:9001/please/give/me/some/money
```

You should recieve a response with status code of 200.

> Tips: when registering the simulation, instead of using the actual pact as payload, you can specify the pact broker URL.

>**Warning: please do not use pact broker's URL if you intend to run performance tests.**

## Example 4: Redirection Simulation

You might at times want to redirect requests. This can be done by using the ```redirect``` registration type:

```
POST http://localhost:8080/v1/simulate?type=redirect&port=9002
```

where the payload contains the URL to which you want to redirect to. So for example if it is Github API:

```
https://api.github.com/
```

After you have done the above registration, if you now do the following post:

```
http://localhost:9002
```

You will get the following response back from Github API:

```json
{
  "current_user_url": "https://api.github.com/user",
  "current_user_authorizations_html_url": "https://github.com/settings/connections/applications{/client_id}",
  "authorizations_url": "https://api.github.com/authorizations",
  "code_search_url": "https://api.github.com/search/code?q={query}{&page,per_page,sort,order}",
  "emails_url": "https://api.github.com/user/emails",
  "emojis_url": "https://api.github.com/emojis",
  "events_url": "https://api.github.com/events",
  "feeds_url": "https://api.github.com/feeds",
  "followers_url": "https://api.github.com/user/followers",
  "following_url": "https://api.github.com/user/following{/target}",
  "gists_url": "https://api.github.com/gists{/gist_id}",
  "hub_url": "https://api.github.com/hub",
  "issue_search_url": "https://api.github.com/search/issues?q={query}{&page,per_page,sort,order}",
  "issues_url": "https://api.github.com/issues",
  "keys_url": "https://api.github.com/user/keys",
  "notifications_url": "https://api.github.com/notifications",
  "organization_repositories_url": "https://api.github.com/orgs/{org}/repos{?type,page,per_page,sort}",
  "organization_url": "https://api.github.com/orgs/{org}",
  "public_gists_url": "https://api.github.com/gists/public",
  "rate_limit_url": "https://api.github.com/rate_limit",
  "repository_url": "https://api.github.com/repos/{owner}/{repo}",
  "repository_search_url": "https://api.github.com/search/repositories?q={query}{&page,per_page,sort,order}",
  "current_user_repositories_url": "https://api.github.com/user/repos{?type,page,per_page,sort}",
  "starred_url": "https://api.github.com/user/starred{/owner}{/repo}",
  "starred_gists_url": "https://api.github.com/gists/starred",
  "team_url": "https://api.github.com/teams",
  "user_url": "https://api.github.com/users/{user}",
  "user_organizations_url": "https://api.github.com/user/orgs",
  "user_repositories_url": "https://api.github.com/users/{user}/repos{?type,page,per_page,sort}",
  "user_search_url": "https://api.github.com/search/users?q={query}{&page,per_page,sort,order}"
}
```
>**Note: this is usefull if you have deployed Phantom to Staging environment where you can switch between simulation and the actual service by performing redirect.**

## Example 5: Logging Simulation

If you want to log the last requests body into a file, they you can use the ```log``` registration type:

```
POST http://localhost:8080/v1/simulate?type=log&port=9003
```

Where the payload is the full path to your log file:

```
C:\logs\log.txt
```

after the above registration is done, the body of the request that comes through port ```9003``` will be logged to the specified log file.

## Example 5: Restore Simulations From Cache

Phantom caches the registered simulations. This allows you to restore individual or all of the simulations through a single post.

For example, if you restarted Phantom but you want to load all simulations from the last run, you can do the following:

```
POST http://localhost:8080/v1/cache?port=0
```

If you want to load the simulation on a single port and you remember which port you can do:

```
POST http://localhost:8080/v1/cache?port=9002
```

Where port 9002 had already had a simulation run previously.

Now if you would like to clear the cache, you can do:

```
POST http://localhost:8080/v1/cache?port=-1
```

For the restoration from cache you don't need to specify any body.

## Tips and Tricks

If you need to auto-generate some IDs or Guids you can easily do that with Phantom. This only applies to the response.

If you have an Id where it needs to be different when the response comes back from Phantom, then you set the value to [INT]. This means that Phantom, will return a random integer, when sending the response back.

Similarly, if you need Phantom to return a response where you require a different GUID, then set the value to [GUID].

So if you register a pact simulation with the following pact:

```
{
  "provider": {
    "name": "Dad"
  },
  "consumer": {
    "name": "Child"
  },
  "interactions": [
    {
      "description": "a request for money",
      "provider_state": "Dad has enough money",
      "request": {
        "method": "post",
        "path": "/please/give/me/some/money",
        "headers": {
          "Content-Type": "application/json; charset=utf-8"
        }
      },
      "response": {
        "status": 200,
        "body": {
          "amount": "[INT]",
          "receipt": "[GUID]"
        }
      }
    }
  ]
}
```
Then the response that comes back everytime will have different values for the amount and the receipt:

```
{
    "status": 200,
    "body": {
      "amount": "19",
      "receipt": "7c4530fd-a689-40db-992b-52fcf4ae983f"
}
```

## Authentication Workaround

At times you might encounter scenarios where the request contains authentication tokens. Phantom automatically removes the following tokens:

```
oauth_consumer_key
oauth_timestamp
oauth_signature
```

## Performance

At the heart of the Phantom is a home made web server using http listener. I have performance tested the web server using Gatling. When the load was ramped up, on a single machine, it steadily handled 4000 requests/second until I ran out of memory on my machine. However, I will leave that to the teams/individuals to decide how many instances they need if it is used during performance testing.
