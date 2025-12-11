# Loki – Quick Overview

**What is Loki?**  
Loki is an open-source **log aggregation system** developed by Grafana Labs. It is designed to **collect, store, and query logs** efficiently, and it works seamlessly with Grafana to visualize log data alongside metrics. Unlike traditional log systems, Loki is optimized for **cost-efficient storage** by indexing only metadata (labels) instead of the full log content.

**Official documentation:** [https://grafana.com/docs/loki/](https://grafana.com/docs/loki/)

---

## Accessing Loki locally

- Loki typically runs as a service in your environment, for example at:

```
http://localhost:3100
```

- Logs can be queried directly via Grafana once Loki is connected.  

---

## Key Features

- **Label-based log organization:** Logs are grouped and filtered using labels instead of full-text indexing.  
- **Seamless Grafana integration:** Allows correlating logs with metrics and traces.  
- **Scalable and efficient:** Optimized for high-volume log storage without high operational cost.  
- **Query logs with LogQL:** Loki provides its own query language for filtering and aggregating log data.

---

## Tips

- Use descriptive labels for your logs (e.g., `app`, `env`, `service`) to make querying easier.  
- Combine metrics from Prometheus with logs from Loki in Grafana dashboards for better observability.  
