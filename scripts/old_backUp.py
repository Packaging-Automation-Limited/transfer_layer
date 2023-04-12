import os
import json
from datetime import datetime, timedelta
from elasticsearch import Elasticsearch
import boto3

# Elasticsearch access details
ELASTICSEARCH_HOST = '35.177.112.190'
ELASTICSEARCH_PORT = 9200
ELASTICSEARCH_INDICES = ['mw-index-hhunter', 'mw-index-primaf', 'mw-index-ftfarms']
ELASTICSEARCH_USERNAME = 'new_admin'
ELASTICSEARCH_PASSWORD = 'new_password'

# AWS access details
AWS_ACCESS_KEY_ID = 'AKIA57TKTI7LNIYHCW4Q'
AWS_SECRET_ACCESS_KEY = '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'
AWS_DEFAULT_REGION = 'eu-west-2'
S3_BUCKET_NAME = 'padatabackup'

# Elasticsearch client
es = Elasticsearch(
        hosts=[{'host': ELASTICSEARCH_HOST, 'port': ELASTICSEARCH_PORT, 'scheme':'http'}],
    http_auth=(ELASTICSEARCH_USERNAME, ELASTICSEARCH_PASSWORD)
)

s3 = boto3.client('s3', aws_access_key_id=AWS_ACCESS_KEY_ID, aws_secret_access_key=AWS_SECRET_ACCESS_KEY, region_name=AWS_DEFAULT_REGION)

backup_dir = '/tmp/es-backup'
#os.mkdir(backup_dir)

for ELASTICSEARCH_INDEX in ELASTICSEARCH_INDICES:
    #BACKUP_PATH = '/tmp'
    export_file = '{}/{}.json'.format(backup_dir, ELASTICSEARCH_INDEX)


    # Create backup filename
    now = datetime.utcnow()
    backup_filename = '{index}_{timestamp}.json'.format(index=ELASTICSEARCH_INDEX, timestamp=now.strftime('%Y%m%d-%H%M%S'))


    # Create backup directory
    backup_directory = os.path.join(backup_dir, ELASTICSEARCH_INDEX)
    os.makedirs(backup_directory, exist_ok=True)

    # Backup Elasticsearch index
    backup_path = os.path.join(backup_directory, backup_filename)

    query = {'query': {'match_all': {}}}
    res = es.search(index=ELASTICSEARCH_INDEX, body=query, scroll='5m', request_timeout=300, size=10000)
    scroll_id = res['_scroll_id']
    hits = res['hits']['hits']
    while len(hits) > 0:
        with open(export_file, 'a') as f:
            for hit in hits:
              f.write('{}\n'.format(json.dumps(hit)))
        res = es.scroll(scroll_id=scroll_id, scroll='5m', request_timeout=300)
        scroll_id = res['_scroll_id']
        hits = res['hits']['hits']
        
     # Upload backup file to S3
    s3_dir = f'elasticsearch-old-backup/{ELASTICSEARCH_INDEX}.json'
    #s3_dir = '{index}/{timestamp}.json'.format(index=ELASTICSEARCH_INDEX, timestamp=now.strftime('%Y%m%d-%H%M%S'))
    s3.upload_file(export_file, S3_BUCKET_NAME, s3_dir)

    # Remove backup file
    #today = datetime.date.today()
    print("Data backed up for", ELASTICSEARCH_INDEX, "of day", now.strftime('%Y%m%d-%H%M%S'))
    #print("Backup completed for ", ELASTICSEARCH_INDEX)
    os.remove(export_file)

