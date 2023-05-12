# AsusDriverFeed
Check ASUS for driver updates and rewrite as an RSS feed.

## Self-host

docker-compose.yml:
```
version: '3.2'

services:
  asus-driver-feed:
    image: ghcr.io/pettijohn/asus-driver-feed:latest
    container_name: asus-driver-feed
    # See below for how to identify these URLs
    environment:
      - DRIVER_URL=https://rog.asus.com/support/webapi/product/GetPDDrivers?website=global&model=GA402XY&pdid=22300&mode=&cpu=GA402XI&osid=52&active=&LevelTagId=161533&systemCode=rog
      - BIOS_URL=https://rog.asus.com/support/webapi/product/GetPDBIOS?website=global&model=GA402XY&pdid=22300&cpu=GA402XI&LevelTagId=161533&systemCode=rog
      - FEED_URL=https://full.url.where.you.host.this/feed.xml
    restart: always
    networks:
      - frontend

# Optional, I prefer each service on its own named network for wiring up to reverse proxy
networks:
  frontend:
```



## How to identify DRIVER_URL and BIOS_URL?

Go to https://rog.asus.com/support

Drill down to Laptops, select your model, and click *Driver & Utility*

Land on a driver page, e.g. https://rog.asus.com/laptops/rog-zephyrus/rog-zephyrus-g14-2023-series/helpdesk_download/

![ASUS driver download page screenshot](docs/driver.png)

Hit F12, Network tab, and select your device & OS. You'll see a network request similar to https://rog.asus.com/support/webapi/product/GetPDDrivers?website=global&model=GA402XY&pdid=22300&mode=&cpu=GA402XI&osid=52&active=&LevelTagId=161533&systemCode=rog

Click Response & verify the data looks like a list of drivers.

![Network inspector of driver details JSON](docs/json.png)

Repeat on the BIOS tab, get a URL like https://rog.asus.com/support/webapi/product/GetPDBIOS?website=global&model=GA402XY&pdid=22300&cpu=GA402XI&LevelTagId=161533&systemCode=rog