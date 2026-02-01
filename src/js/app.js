const Request = window.Request
const Headers = window.Headers
const fetch = window.fetch

class Api {
  async request (method, url, body) {
    if (body) {
      body = JSON.stringify(body)
    }

    const request = new Request('/api/' + url, {
      method: method,
      body: body,
      credentials: 'same-origin',
      headers: new Headers({
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      })
    })

    const resp = await fetch(request)
    const jsonResult = await resp.json()

    if (!resp.ok) {
      if (resp.status === 400) {
        jsonResult.requestStatus = 400
      } else if (resp.status === 500) {
        jsonResult.requestStatus = 500
      }
      throw new Error(jsonResult.error || resp.statusText)
    }

    return jsonResult
  }

  async getDomain (domainOrIp) {
    return this.request('GET', `domain/${encodeURIComponent(domainOrIp)}`)
  }
}

const api = new Api()

class DomainValidator {
  static isValid(domain) {
    if (!domain || typeof domain !== 'string') {
      return false
    }

    const trimmed = domain.trim()
    
    if (!trimmed || !trimmed.includes('.')) {
      return false
    }

    const domainRegex = /^([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,}$/i
    return domainRegex.test(trimmed)
  }

  static normalize(domain) {
    if (!domain) return ''
    return domain.trim().toLowerCase()
  }
}

class DomainView {
  constructor(containerId) {
    this.container = document.getElementById(containerId)
  }

  showLoading() {
    this.container.innerHTML = `
      <div class="alert alert-info">
        <i class="icon icon-spinner icon-spin"></i> Pesquisando informações do domínio...
      </div>
    `
    this.container.classList.remove('hide')
  }

  showError(message) {
    this.container.innerHTML = `
      <div class="alert alert-danger">
        <strong>Erro:</strong> ${this.escapeHtml(message)}
      </div>
    `
    this.container.classList.remove('hide')
  }

  showDomainInfo(domain) {
    if (!domain) {
      this.showError('Nenhuma informação encontrada para este domínio.')
      return
    }

    let html = `
      <div class="card">
        <div class="card-header bg-primary text-white">
          <h4 class="mb-0">
            <i class="icon icon-globe"></i> Informações do Domínio: ${this.escapeHtml(domain.name)}
          </h4>
        </div>
        <div class="card-body">
          <div class="row">
            <div class="col-md-6">
              <h5>Informações DNS</h5>
              <table class="table table-striped">
                <tbody>
                  ${domain.ip ? `
                    <tr>
                      <th scope="row">Endereço IP (A)</th>
                      <td><code>${this.escapeHtml(domain.ip)}</code></td>
                    </tr>
                  ` : ''}
                  ${domain.nameServers ? `
                    <tr>
                      <th scope="row">Name Servers</th>
                      <td><code>${this.escapeHtml(domain.nameServers)}</code></td>
                    </tr>
                  ` : ''}
                  ${domain.hostedAt ? `
                    <tr>
                      <th scope="row">Hospedado em</th>
                      <td><strong>${this.escapeHtml(domain.hostedAt)}</strong></td>
                    </tr>
                  ` : ''}
                </tbody>
              </table>
            </div>
            <div class="col-md-6">
              <h5>Informações WHOIS</h5>
              ${domain.whoIs ? `
                <div class="whois-data">
                  <pre class="bg-light p-3" style="max-height: 400px; overflow-y: auto; font-size: 0.85em;">${this.escapeHtml(domain.whoIs)}</pre>
                </div>
              ` : '<p class="text-muted">Informações WHOIS não disponíveis.</p>'}
            </div>
          </div>
        </div>
      </div>
    `

    this.container.innerHTML = html
    this.container.classList.remove('hide')
  }

  hide() {
    this.container.classList.add('hide')
  }

  escapeHtml(text) {
    if (!text) return ''
    const div = document.createElement('div')
    div.textContent = text
    return div.innerHTML
  }
}

var callback = () => {
  const btn = document.getElementById('btn-search')
  const txt = document.getElementById('txt-search')
  const resultView = new DomainView('whois-results')

  if (btn && txt) {
    const searchDomain = async () => {
      const domain = DomainValidator.normalize(txt.value)

      if (!DomainValidator.isValid(domain)) {
        resultView.showError('Por favor, informe um domínio válido (ex: exemplo.com)')
        return
      }

      btn.disabled = true
      btn.innerHTML = '<i class="icon icon-spinner icon-spin"></i> Pesquisando...'
      
      resultView.showLoading()

      try {
        const response = await api.getDomain(domain)
        resultView.showDomainInfo(response)
      } catch (error) {
        resultView.showError(error.message || 'Erro ao consultar informações do domínio.')
      } finally {
        btn.disabled = false
        btn.innerHTML = '<i class="icon icon-search icon-white mr-1"></i><span>Pesquisar...</span>'
      }
    }

    btn.onclick = searchDomain

    txt.addEventListener('keypress', (e) => {
      if (e.key === 'Enter') {
        searchDomain()
      }
    })
  }
}

if (document.readyState === 'complete' || (document.readyState !== 'loading' && !document.documentElement.doScroll)) {
  callback()
} else {
  document.addEventListener('DOMContentLoaded', callback)
}
