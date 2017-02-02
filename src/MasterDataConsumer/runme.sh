sleep 20s
echo ">>> Making sure MySQL is up"
/bin/bash wait-for-it.sh db:3306 -t 0
echo "<<< Done checking on MySQL"

echo ">>> Making sure Kafka is up"
/bin/bash wait-for-it.sh kafka:9092 -t 0
echo "<<< Done checking on Kafka"

sleep 60s

dotnet MasterDataConsumer.dll
