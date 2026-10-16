import React, { useState, useEffect } from 'react';
import automationService from '../services/automation-service';
import './Dashboard.css';

function Dashboard({ projectPath }) {
  const [features, setFeatures] = useState([]);
  const [loading, setLoading] = useState(false);
  const [filter, setFilter] = useState('');
  const [selectedTag, setSelectedTag] = useState('');
  const [hoveredFeatureId, setHoveredFeatureId] = useState(null);
  const [stats, setStats] = useState({
    total: 0,
    totalScenarios: 0,
    byTag: {}
  });

  useEffect(() => {
    if (projectPath) {
      loadFeatures();
    }
  }, [projectPath]);

  const loadFeatures = async () => {
    setLoading(true);
    try {
      const result = await automationService.scanFeatures(projectPath);
      if (result.success) {
        setFeatures(result.data);
        calculateStats(result.data);
      }
    } catch (error) {
      console.error('Erro ao carregar features:', error);
    } finally {
      setLoading(false);
    }
  };

  const calculateStats = (featuresList) => {
    const statsData = {
      total: featuresList.length,
      totalScenarios: 0,
      byTag: {}
    };

    featuresList.forEach(feature => {
      // Conta cenários
      statsData.totalScenarios += feature.scenarios?.length || 0;
      
      // Conta tags
      feature.tags.forEach(tag => {
        statsData.byTag[tag] = (statsData.byTag[tag] || 0) + 1;
      });
    });

    setStats(statsData);
  };

  const filteredFeatures = features.filter(feature => {
    const matchesFilter = !filter || 
      feature.name.toLowerCase().includes(filter.toLowerCase()) ||
      feature.description.toLowerCase().includes(filter.toLowerCase());
    
    const matchesTag = !selectedTag || feature.tags.includes(selectedTag);
    
    return matchesFilter && matchesTag;
  });

  const allTags = [...new Set(features.flatMap(f => f.tags))];

  return (
    <div className="dashboard">
      <div className="dashboard-header">
        <h2>Dashboard de Automações</h2>
        <button onClick={loadFeatures} className="btn-refresh" disabled={loading}>
          {loading ? '⏳ Carregando...' : '🔄 Atualizar'}
        </button>
      </div>

      <div className="dashboard-stats">
        <div className="stat-card">
          <div className="stat-value">{stats.total}</div>
          <div className="stat-label">Total de Features</div>
        </div>
        <div className="stat-card">
          <div className="stat-value">{stats.totalScenarios || 0}</div>
          <div className="stat-label">Total de Cenários</div>
        </div>
        <div className="stat-card">
          <div className="stat-value">{allTags.length}</div>
          <div className="stat-label">Tags Únicas</div>
        </div>
      </div>

      <div className="dashboard-filters">
        <input
          type="text"
          placeholder="🔍 Buscar features..."
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
          className="filter-input"
        />
        <select
          value={selectedTag}
          onChange={(e) => setSelectedTag(e.target.value)}
          className="filter-select"
        >
          <option value="">Todas as tags</option>
          {allTags.map(tag => (
            <option key={tag} value={tag}>{tag}</option>
          ))}
        </select>
      </div>

      <div className="features-grid">
        {loading ? (
          <div className="loading">Carregando features...</div>
        ) : filteredFeatures.length === 0 ? (
          <div className="empty-state">
            <p>Nenhuma feature encontrada.</p>
            {projectPath && <p>Verifique se o projeto está configurado corretamente.</p>}
          </div>
        ) : (
          filteredFeatures.map(feature => (
            <div key={feature.path} className="feature-card">
              <div className="feature-header">
                <div className="feature-tags-container">
                  <div className="feature-tags">
                    {feature.tags.slice(0, 3).map(tag => (
                      <span key={tag} className="tag" title={tag}>{tag}</span>
                    ))}
                    {feature.tags.length > 3 && (
                      <div 
                        className="tag-more-wrapper"
                        onMouseEnter={() => setHoveredFeatureId(feature.path)}
                        onMouseLeave={() => setHoveredFeatureId(null)}
                        onClick={(e) => {
                          e.stopPropagation();
                          setHoveredFeatureId(hoveredFeatureId === feature.path ? null : feature.path);
                        }}
                      >
                        <span className="tag tag-more" title={`Clique para ver ${feature.tags.length - 3} tag(s) oculta(s)`}>
                          +{feature.tags.length - 3}
                        </span>
                        {hoveredFeatureId === feature.path && (
                          <div 
                            className="tags-tooltip"
                            onMouseEnter={() => setHoveredFeatureId(feature.path)}
                            onMouseLeave={() => setHoveredFeatureId(null)}
                            onClick={(e) => e.stopPropagation()}
                          >
                            <div className="tags-tooltip-content">
                              <div className="tags-tooltip-title">Tags ({feature.tags.length - 3}):</div>
                              <div className="tags-tooltip-list">
                                {feature.tags.slice(3).map(tag => (
                                  <span key={tag} className="tag tag-tooltip" title={tag}>{tag}</span>
                                ))}
                              </div>
                            </div>
                          </div>
                        )}
                      </div>
                    )}
                  </div>
                </div>
                <h3>{feature.name}</h3>
              </div>
              {feature.description && (
                <p className="feature-description">{feature.description}</p>
              )}
              <div className="feature-meta">
                <span>📄 {feature.scenarios?.length || 0} cenário(s)</span>
                <span>📅 {new Date(feature.lastModified).toLocaleDateString('pt-BR')}</span>
              </div>
              <div className="feature-path">{feature.path}</div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}

export default Dashboard;

