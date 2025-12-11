# Prometheus – Quick Overview

**What is Prometheus?**  
Prometheus is an open-source **monitoring and alerting toolkit** designed for **reliably collecting metrics from applications and infrastructure**. It stores time-series data, supports powerful queries with its own language (PromQL), and integrates well with Grafana for visualization and alerting.

**Official documentation:** [https://prometheus.io/docs/](https://prometheus.io/docs/)

---

## Accessing Prometheus locally

- Prometheus typically runs as a service in your environment, for example at:

```
http://localhost:3100
```

- You can explore metrics directly using the Prometheus web interface.

---

## Key Features

- **Multi-dimensional data model:** Metrics are stored with key/value labels for flexible querying.  
- **Powerful query language (PromQL):** Enables complex aggregations, filters, and calculations.  
- **Pull-based metrics collection:** Prometheus scrapes metrics endpoints from monitored targets at regular intervals.  
- **Alerting support:** Integrates with Alertmanager to trigger alerts based on metric conditions.  

---

## Tips

- Expose metrics from your applications (e.g., `/metrics` endpoint) to be scraped by Prometheus.  
- Combine metrics from Prometheus with logs from Loki and dashboards in Grafana for full observability.  
