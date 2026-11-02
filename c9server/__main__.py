import argparse
import asyncio
from pathlib import Path

from .server import ResearchServer, ServerConfig


async def serve(config):
    server = await ResearchServer(config).start()
    try:
        await server.listener.serve_forever()
    finally:
        await server.close()


def main():
    parser = argparse.ArgumentParser(description='Experimental local C9 TCP server; original-client compatibility is unverified')
    parser.add_argument('--config', type=Path)
    parser.add_argument('--host', help='Numeric loopback address')
    parser.add_argument('--port', type=int)
    args = parser.parse_args()
    try:
        config = ServerConfig.load(args.config, host=args.host, port=args.port) if args.config else ServerConfig(
            host=args.host or '127.0.0.1', port=args.port if args.port is not None else 14000)
        asyncio.run(serve(config))
    except (ValueError, OSError, RuntimeError) as error:
        parser.exit(2, f'c9server: {error}\n')
    except KeyboardInterrupt:
        pass


if __name__ == '__main__':
    main()
