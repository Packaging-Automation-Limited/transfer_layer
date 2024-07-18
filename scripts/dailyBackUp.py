import os
import json
from datetime import datetime, timedelta
from elasticsearch import Elasticsearch
import boto3

# Elasticsearch access details
ELASTICSEARCH_HOST = '35.177.112.190'
ELASTICSEARCH_PORT = 9200
ELASTICSEARCH_INDICES = ['mw-index-hhunter',
                         'mw-index-primaf', 'mw-index-ftfarms']
ELASTICSEARCH_USERNAME = ''
ELASTICSEARCH_PASSWORD = ''

# AWS access details
AWS_ACCESS_KEY_ID = 'KEY_ID'
AWS_SECRET_ACCESS_KEY = 'ADD_KEY'
AWS_DEFAULT_REGION = 'eu-west-2'
S3_BUCKET_NAME = 'padatabackup'


def get_indices_data():
    es = Elasticsearch(
        hosts=[{'host': ELASTICSEARCH_HOST,
                'port': ELASTICSEARCH_PORT, 'scheme': 'http'}],
        http_auth=(ELASTICSEARCH_USERNAME, ELASTICSEARCH_PASSWORD))

    # indices = ['index1', 'index2']  # Replace with your index names
    data = {}
    for index in ELASTICSEARCH_INDICES:
        json_query = {
            "query": {
                "range": {
                    "@timestamp": {
                        "gte": "now-24h",
                        "lte": "now"
                    }
                }
            }
        }
        response = es.search(
            index=index,
            scroll='5m',
            request_timeout=300,
            size=10000,
            body=json_query
        )
        hits = response['hits']['hits']
        data[index] = hits
    return data


def upload_to_s3(data):
    s3 = boto3.client(
        's3',
        aws_access_key_id=AWS_ACCESS_KEY_ID,
        aws_secret_access_key=AWS_SECRET_ACCESS_KEY
    )
    for index, hits in data.items():
        s3_key = f'elasticsearch-backup/{index}/{datetime.now().strftime("%Y-%m-%d")}.json'
        s3.put_object(Body=json.dumps(hits), Bucket='padatabackup', Key=s3_key)
        print("Data dumped for", index, "of day",
              datetime.now().strftime("%Y-%m-%d"))


if __name__ == '__main__':
    data = get_indices_data()
    # print("Data recvd")
    # print(data)
    upload_to_s3(data)
