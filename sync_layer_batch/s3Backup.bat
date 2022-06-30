#!/bin/bash
set client_id=mw_pal_<ADD CLIENT ID HERE>
set local_dir_path=C:/S3SyncedFiles
aws s3 sync s3://%client_id% %local_dir_path%
aws s3 rm s3://%client_id% --recursive
