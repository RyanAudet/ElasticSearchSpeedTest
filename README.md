# Lambda and Elastic Search

Lambda is taking a lot longer to work with elastic search than local

## Steps to reproduce

- Deploy the project to AWS lambda
- Find `elasticsearchspeedtest` in your AWS Elastic Search instances
- Wait until Domain Status is set to Active and copy the Endpoint URL
- Add user secrets for Elastic Search Endpoint URL

```json
{
  "ElasticSearch": {
    "URL": "https://<ElasticSearchEndpointURL>/"
  }
}
```

- Start the application locally
- Generate test data by triggering url by going to API url + /swagger and posing to /api/Search
