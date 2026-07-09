# Tyler MCP Security Overlay Compliance Checklist - v1.4.0

*Based on: [Tyler MCP Security Overlay (Normative) - v1.4.0](https://tylertech.atlassian.net/wiki/x/OwTCMw)*
*Last Updated: 2026-03-19*

---

## Instructions

This checklist is derived from the Tyler MCP Security Overlay (Normative) - v1.4.0 document. Each item preserves the original requirement language (MUST, SHOULD, MAY, SHALL NOT, etc.) from RFC 2119.

- **MUST / MUST NOT / SHALL NOT**: Mandatory requirements - all must be checked for compliance
- **SHOULD / SHOULD NOT**: Strongly recommended - document justification if not implemented
- **MAY**: Optional - implement based on your use case

**Submission Process:**
1. Rename this file following the naming convention: `[PRODUCT]_MCP_Server_Compliance_Checklist-v1.4.0.md` (e.g., `ERPPRO_MCP_Server_Compliance_Checklist-v1.4.0.md`)
2. Complete all applicable checkboxes throughout the document
3. Upload the completed checklist to `/docs/compliance/` in your MCP server repository (version controlled alongside the MCP code)
4. Fill in the Compliance Summary section at the end with counts, reviewer information, and product details
5. Submit a link to the completed checklist in your repository to the Tyler Foundry Team

**Repository Location**: `/docs/compliance/[PRODUCT]_MCP_Server_Compliance_Checklist-v1.4.0.md`

Failure to comply with binding requirements prohibits designation of the MCP server as Tyler-certified or approved for external customer use.

**Companion Documents:**
- [Tyler MCP Technical Profile](https://tylertech.atlassian.net/wiki/x/4QDgMw) (technical interoperability requirements)
- [Tyler MCP Implementation Guidance](https://tylertech.atlassian.net/wiki/x/94bUMw) (non-normative patterns and examples)
- [Tyler MCP External Client Integration Profile](https://tylertech.atlassian.net/wiki/x/lwMqN) (external third-party API access requirements)

**MCP Specification Compliance:**
- [ ] MUST comply with the official MCP specification

---

## 1. Identity and Access Security

**Reference:** Implementation guidance for authentication patterns, token validation, and tenant context binding is available in the [Tyler MCP Implementation Guidance](https://tylertech.atlassian.net/wiki/x/94bUMw).

### Authentication Requirements

- [ ] MUST require authentication for all requests
- [ ] SHOULD NOT allow anonymous access without explicit Corporate Legal and Corporate Security approval

#### If Anonymous Access is Permitted:

- [ ] MUST NOT expose tenant-specific, client production, or regulated data
- [ ] MUST be disabled by default in production deployments
- [ ] Exception: Public domain endpoints MAY allow anonymous access with Corporate Legal and Corporate Security approval

### Token & JWT Validation

- [ ] MUST validate JSON Web Tokens (JWT) in accordance with Tyler Identity standards, RFC 7519, and OAuth 2.1 resource server requirements
- [ ] MUST enforce token revocation checks where supported by Tyler Identity
- [ ] MUST NOT transmit TID tokens to downstream non-Tyler services

### Authorization & Tenant Controls

- [ ] MUST adhere to the principle of least privilege
- [ ] MUST enforce authorization consistent with the underlying product's authorization model
- [ ] MUST ensure authenticated requests are constrained to the user's authorized tenant context
- [ ] MUST enforce applicable product licensing constraints for the authenticated tenant or user
- [ ] MUST NOT expose tools, resources, or prompts that are not licensed for the authenticated tenant or user

---

## 2. Transport and Network Security

- [ ] MUST use TLS 1.2 or higher for all HTTP communications
- [ ] SHOULD use Mutual TLS (mTLS) where supported by the deployment environment
- [ ] MUST disable insecure or deprecated cipher suites consistent with Tyler enterprise cryptographic standards
- [ ] MUST be deployed behind Tyler-approved network controls
- [ ] MUST NOT expose administrative endpoints publicly
- [ ] MUST be deployed behind a Web Application Firewall (WAF) for defense-in-depth protection against common attack vectors

**WAF Reference:**
- For general AWS WAF setup: [Corp Cloud Hosting - AWS WAF Configuration & Deployment Guide](https://tylertech.atlassian.net/wiki/x/HQRMFQ)
- For MCP-specific WAF guidance: [Tyler MCP Implementation Guidance](https://tylertech.atlassian.net/wiki/x/94bUMw)

**WAF Configuration (Check Applicable Section):**

#### If Exposed Externally:

- [ ] WAF MUST be configured with rules in BLOCK mode per the Tyler MCP External Client Integration Profile
- [ ] SHOULD be routed through the Tyler MCP Gateway (will turn MUST when such infrastructure is designated as production-ready by CTO Office)
- [ ] Prior to Gateway availability, any externally exposed MCP server MUST receive explicit Corporate Security approval

#### If Internal-Only (accessed only by Tyler Foundry or internal services):

- [ ] WAF MUST be configured with rules in COUNT mode (monitoring/alerting without blocking) as a minimum baseline

---

## 3. Multi-Tenancy and Isolation

**Reference:** Guidance for tenant isolation, context validation, and tenant identifier mapping is available in the [Tyler MCP Implementation Guidance](https://tylertech.atlassian.net/wiki/x/94bUMw).

- [ ] MUST enforce strict tenant isolation
- [ ] MUST propagate tamper-proof tenant context through downstream calls
- [ ] MUST prevent cross-tenant resource access
- [ ] MUST validate tenant context on every request
- [ ] MUST include tenant identifier in logs for all tenant-scoped operations

---

## 4. Tool and Resource Security Controls

- [ ] MUST enforce namespace compliance (tyl_*)
- [ ] MUST enforce runtime validation of tool invocation inputs against the declared JSON Schema prior to execution
- [ ] MUST restrict destructive tools (delete, void, approve, submit) via authorization checks
- [ ] SHOULD support configurable human-in-the-loop approval for high-risk operations
- [ ] MUST prevent bulk export operations (e.g., operations returning >1000 records, >10MB of data, or unbounded result sets) without explicit authorization validation
- [ ] MUST limit tool exposure to only those the authenticated user can access

---

## 5. Logging, Observability, and Audit

### Observability Integration

- [ ] SHOULD integrate with Tyler OpenTelemetry collectors or a Tyler-approved centralized observability pipeline prior to production approval

### Log Fields

SHOULD include the following fields in logs:

- [ ] correlation_id (or equivalent request identifier)
- [ ] user identifier (e.g., userId)
- [ ] tenant identifier (e.g., tenantId)
- [ ] session_id (if applicable)

### Logging Requirements

- [ ] MUST NOT log full request or response bodies containing sensitive data unless explicitly required and approved under enterprise logging standards
- [ ] SHOULD generate a correlation_id if one is not present

### Operational Endpoints

- [ ] SHOULD implement a /health endpoint for operational monitoring following emerging Tyler health check format conventions
- [ ] SHOULD include Retry-After header for rate limit errors to improve client retry behavior
- [ ] MUST NOT expose internal system details in error messages to prevent information disclosure

**Reference:** Guidance for tenant identifier mapping and rate limiting patterns is available in the [Tyler MCP Implementation Guidance](https://tylertech.atlassian.net/wiki/x/94bUMw).

---

## 6. Secure Development and Vulnerability Management

### Security Scanning Requirements

- [ ] MUST comply with the Tyler Dynamic and Static Code Vulnerability Scanning Policy, including required SAST (Static Application Security Testing) prior to production approval
- [ ] MUST comply with the Tyler Dynamic and Static Code Vulnerability Scanning Policy, including required DAST (Dynamic Application Security Testing) prior to production approval
- [ ] MUST pass approved dependency vulnerability scanning prior to production approval
- [ ] MUST enforce CI/CD gating such that builds or releases are blocked on unresolved Critical or High vulnerabilities unless formally risk-accepted in accordance with the Tyler Application Vulnerability Remediation Policy
- [ ] MUST NOT allow production deployment with unresolved Critical or High severity vulnerabilities as defined by the Tyler Application Vulnerability Remediation Policy
- [ ] MUST undergo re-scan upon major version updates

### Secrets Management

- [ ] MUST manage, store, and protect secrets in accordance with Tyler Secrets Policy

### Security Testing

- [ ] MUST be tested against the OWASP LLM Top 10 risks
- [ ] MUST document outbound network dependencies prior to production deployment

### Environment Controls

- [ ] Development and sandbox environments MUST be deployed in isolated, non-production environments logically and network-segmented from production systems in accordance with Tyler's Secure Application Development Program Policy
- [ ] Sandbox environments MUST NOT use production or regulated data unless explicitly approved
- [ ] Sensitive data (including PII and CJIS-regulated data) MUST be anonymized, tokenized, or otherwise sanitized/redacted prior to use in MCP server development or testing environments
- [ ] Destructive or high-impact operations in sandbox environments MUST include appropriate safeguards to limit unintended side effects and blast radius

---

## 7. Third-Party Integration Security

### Preview Restriction

🔴 **Preview Restriction:** Integration with third-party services is not permitted during the Tyler AI Studio Private Preview phase.

- [ ] No third-party integrations during Private Preview

### GA Deployment Requirements

For GA deployments:

- [ ] MCP servers SHALL NOT integrate with third-party services without explicit Corporate Cloud Services and Corporate Security approval
- [ ] Approved third-party integrations MUST comply with all requirements defined in this section

### External API Requirements

If MCP servers call external APIs:

- [ ] MUST document all outbound calls
- [ ] MUST minimize data transmitted externally
- [ ] MUST NOT transmit TID tokens or Authorization headers containing identity tokens to non-Tyler services
- [ ] MUST isolate third-party credentials using approved secret management solutions in accordance with Tyler Secrets Policy
- [ ] MUST restrict outbound network calls to approved domains and endpoints via allowlists and/or network policy controls
- [ ] MUST implement OAuth 2.1 with PKCE when acting as an OAuth client to external services
