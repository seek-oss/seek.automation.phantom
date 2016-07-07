### 0.7.1
	* When matching the request against the available requests in the pact file, both path and query are now matched. 
	  However, please note that the oAuth parameters ("oauth_consumer_key", "oauth_timestamp", "oauth_signature") are filtered out of the query. 
	  If there are other parameters that needs to get filtered out, please update the Helper class.

### 0.7.0
	* Add the ability to clear the cache

### 0.6.0
	* ApplyStaticRules for more registration types
	* If [INT] is found in the payload then it will be replaced with a random integer

### 0.5.0
	* Bug: fixed an issue with caching where registrations on the same port was being kept for all the different types of registration resulting on random load
	* Feature: add the ability to register a logging on a port to write the last request to a port

### 0.4.0
	* Updated to let the service open the windows ports between 9000 and 9025 inclusive on startup

### 0.3.0
	* Bug Fix: if the body didn't have any content then Simulator was throwing exception. Fixed it by: 
		* Getting the callbacks to return HTTPResponseMessage instead of the dull string
		
### 0.2.0
	* Added the endpoints to reload from Cache
	* Added the endpoint to list the occupied ports

### 0.1.0
	* Created