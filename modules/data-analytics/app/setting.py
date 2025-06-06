import os

from utils import get_from_env, str_to_bool


WSGI = get_from_env("WSGI", False, type_cast=str_to_bool)

TEST = get_from_env("TEST", True, type_cast=str_to_bool)
SUFFIX = os.getenv("SUFFIX", "")

KAFKA_HOSTS = os.getenv("KAFKA_HOSTS", "localhost:29092")
KAFKA_SECURITY_PROTOCOL = os.getenv("KAFKA_SECURITY_PROTOCOL", None)
KAFKA_SASL_MECHANISM = os.getenv("KAFKA_SASL_MECHANISM", None)
KAFKA_SASL_USER = os.getenv("KAFKA_SASL_USER", None)
KAFKA_SASL_PASSWORD = os.getenv("KAFKA_SASL_PASSWORD", None)
KAFKA_PRODUCER_RETRIES = 3
KAFKA_PRODUCER_ENABLED = get_from_env("KAFKA_PRODUCER_ENABLED", True, type_cast=str_to_bool)
KAFKA_PREFIX = os.getenv("KAFKA_PREFIX", "")

CLICKHOUSE_HOST = os.getenv("CLICKHOUSE_HOST", "localhost")
CLICKHOUSE_ALT_HOST = os.getenv("CLICKHOUSE_ALT_HOST", None)
CLICKHOUSE_PORT = get_from_env("CLICKHOUSE_PORT", 9000, type_cast=int)
CLICKHOUSE_HTTP_PORT = get_from_env("CLICKHOUSE_HTTP_PORT", 8123, type_cast=int)
CLICKHOUSE_CLUSTER = os.getenv("CLICKHOUSE_CLUSTER", "featbit_ch_cluster")
CLICKHOUSE_DATABASE = os.getenv("CLICKHOUSE_DATABASE", "featbit") + SUFFIX
CLICKHOUSE_SECURE = get_from_env("CLICKHOUSE_SECURE", False, type_cast=str_to_bool)
CLICKHOUSE_USER = os.getenv("CLICKHOUSE_USER", "default")
CLICKHOUSE_PASSWORD = os.getenv("CLICKHOUSE_PASSWORD", "")
CLICKHOUSE_CA = os.getenv("CLICKHOUSE_CA", None)
CLICKHOUSE_VERIFY = get_from_env("CLICKHOUSE_VERIFY", True, type_cast=str_to_bool)
CLICKHOUSE_CONN_POOL_MIN = get_from_env("CLICKHOUSE_CONN_POOL_MIN", 20, type_cast=int)
CLICKHOUSE_CONN_POOL_MAX = get_from_env("CLICKHOUSE_CONN_POOL_MAX", 1000, type_cast=int)
CLICKHOUSE_ENABLE_STORAGE_POLICY = get_from_env("CLICKHOUSE_ENABLE_STORAGE_POLICY", False, type_cast=str_to_bool)
CLICKHOUSE_KAFKA_HOSTS = os.getenv("CLICKHOUSE_KAFKA_HOSTS", "kafka:9092")
CLICKHOUSE_REPLICATION = get_from_env("CLICKHOUSE_REPLICATION", True, type_cast=str_to_bool)

DB_PROVIDER = os.getenv("DB_PROVIDER", "Postgres")

POSTGRES_CONNECTION_STRING = os.getenv("POSTGRES_CONNECTION_STRING", "")
POSTGRES_USER = os.getenv("POSTGRES_USER", "postgres")
POSTGRES_PASSWORD = os.getenv("POSTGRES_PASSWORD", "please_change_me")
POSTGRES_HOST = os.getenv("POSTGRES_HOST", "localhost")
POSTGRES_PORT = os.getenv("POSTGRES_PORT", "5432")
POSTGRES_DATABASE = os.getenv("POSTGRES_DATABASE", "featbit")

CACHE_KEY_PREFIX = "featbit:das:"

MONGO_URI = os.getenv("MONGO_URI", "mongodb://admin:password@localhost:27017")
MONGO_DB = os.getenv("MONGO_INITDB_DATABASE", "featbit")
MONGO_DB_EVENTS_COLLECTION = "Events"

SHELL_PLUS_PRINT_SQL = True if TEST else False

DATE_ISO_FMT = '%Y-%m-%dT%H:%M:%S.%f'

DATE_SIM_FMT = '%Y-%m-%d %H:%M:%S'
DATE_UTC_FMT = '%Y-%m-%dT%H:%M:%SZ'

DEFAULT_LOGGING_CONFIG = {
    'version': 1,
    'disable_existing_loggers': False,
    'formatters': {'default': {
        'format': '[%(asctime)s] %(levelname)s in %(module)s: %(message)s',
    }},
    'handlers': {'wsgi': {
        'class': 'logging.StreamHandler',
        'stream': 'ext://flask.logging.wsgi_errors_stream',
        'formatter': 'default'
    }},
    'root': {
        'level': 'INFO',
        'handlers': ['wsgi']
    }
}

ENABLE_OPENTELEMETRY = get_from_env("ENABLE_OPENTELEMETRY", False, type_cast=str_to_bool)
