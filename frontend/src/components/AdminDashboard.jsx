import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import {
  Container,
  Paper,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Button,
  Select,
  MenuItem,
  FormControl,
  Box,
  Alert,
  Tabs,
  Tab,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Snackbar,
  CircularProgress,
  RadioGroup,
  FormControlLabel,
  Radio,
  Switch
} from '@mui/material';

const AdminDashboard = () => {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState(0);
  const [users, setUsers] = useState([]);
  const [games, setGames] = useState([]);
  const [categories, setCategories] = useState([]);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(true);
  const [openDialog, setOpenDialog] = useState(false);
  const [dialogType, setDialogType] = useState('');
  const [selectedItem, setSelectedItem] = useState(null);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    imageUrl: '',
    gameUrl: '',
    categoryId: '',
    isActive: true,
    playCount: 0,
    lastPlayed: null,
    createdAt: new Date(),
    updatedAt: null
  });
  const [selectedImage, setSelectedImage] = useState(null);
  const [selectedGameFile, setSelectedGameFile] = useState(null);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [imageUploadMethod, setImageUploadMethod] = useState('url');
  const [gameUploadMethod, setGameUploadMethod] = useState('url');

  useEffect(() => {
    if (user?.role === 'Admin') {
      fetchData();
    }
  }, [user, activeTab]);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError('');
      const [usersRes, gamesRes, categoriesRes] = await Promise.all([
        fetch('http://localhost:5000/api/admin/users', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        }),
        fetch('http://localhost:5000/api/game', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        }),
        fetch('http://localhost:5000/api/category', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        })
      ]);

      if (!usersRes.ok) {
        const errorData = await usersRes.json();
        throw new Error(errorData.message || 'Failed to fetch users');
      }
      if (!gamesRes.ok) {
        const errorData = await gamesRes.json();
        throw new Error(errorData.message || 'Failed to fetch games');
      }
      if (!categoriesRes.ok) {
        const errorData = await categoriesRes.json();
        throw new Error(errorData.message || 'Failed to fetch categories');
      }

      const [usersData, gamesData, categoriesData] = await Promise.all([
        usersRes.json(),
        gamesRes.json(),
        categoriesRes.json()
      ]);

      setUsers(usersData);
      setGames(gamesData);
      setCategories(categoriesData);
    } catch (err) {
      console.error('Error fetching data:', err);
      setError('Failed to load data: ' + (err.message || 'Unknown error'));
    } finally {
      setLoading(false);
    }
  };

  const handleTabChange = (event, newValue) => {
    setActiveTab(newValue);
  };

  const handleOpenDialog = (type, item = null) => {
    setDialogType(type);
    setSelectedItem(item);
    if (item) {
      setFormData({
        name: item.name || '',
        description: item.description || '',
        imageUrl: item.imageUrl || '',
        gameUrl: item.gameUrl || '',
        categoryId: item.categoryId || '',
        isActive: item.isActive ?? true,
        playCount: item.playCount || 0,
        lastPlayed: item.lastPlayed || null,
        createdAt: item.createdAt || new Date(),
        updatedAt: item.updatedAt || null
      });
    } else {
      setFormData({
        name: '',
        description: '',
        imageUrl: '',
        gameUrl: '',
        categoryId: '',
        isActive: true,
        playCount: 0,
        lastPlayed: null,
        createdAt: new Date(),
        updatedAt: null
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setSelectedItem(null);
    setFormData({
      name: '',
      description: '',
      imageUrl: '',
      gameUrl: '',
      categoryId: '',
      isActive: true,
      playCount: 0,
      lastPlayed: null,
      createdAt: new Date(),
      updatedAt: null
    });
  };

  const handleCloseSnackbar = () => {
    setError('');
    setSuccess('');
  };

  const handleImageChange = (event) => {
    if (event.target.files && event.target.files[0]) {
      setSelectedImage(event.target.files[0]);
    }
  };

  const handleGameFileChange = (event) => {
    if (event.target.files && event.target.files[0]) {
      setSelectedGameFile(event.target.files[0]);
    }
  };

  const uploadFile = async (file, type) => {
    try {
      const formData = new FormData();
      formData.append('file', file);

      const endpoint = type === 'image' ? 'image' : 'game';
      const response = await fetch(`http://localhost:5000/api/upload/${endpoint}`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: formData
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Upload failed');
      }

      const data = await response.json();
      return data.path;
    } catch (error) {
      setError('Failed to upload file: ' + error.message);
      return null;
    }
  };

  const handleSubmit = async () => {
    try {
      setError('');
      let url = '';
      let method = '';
      let body = {};

      if (dialogType === 'game') {
        url = selectedItem
          ? `http://localhost:5000/api/game/${selectedItem.id}`
          : 'http://localhost:5000/api/game';
        method = selectedItem ? 'PUT' : 'POST';

        // Upload game file if selected
        let gameUrl = formData.gameUrl;
        if (selectedGameFile) {
          gameUrl = await uploadFile(selectedGameFile, 'game');
          if (!gameUrl) return;
        }

        // Upload image if selected
        let imageUrl = formData.imageUrl;
        if (selectedImage) {
          imageUrl = await uploadFile(selectedImage, 'image');
          if (!imageUrl) return;
        }

        body = {
          name: formData.name,
          description: formData.description,
          imageUrl: imageUrl,
          gameUrl: gameUrl,
          categoryId: parseInt(formData.categoryId),
          isActive: formData.isActive
        };
      } else if (dialogType === 'category') {
        url = selectedItem
          ? `http://localhost:5000/api/category/${selectedItem.id}`
          : 'http://localhost:5000/api/category';
        method = selectedItem ? 'PUT' : 'POST';

        // Upload image if selected
        let imageUrl = formData.imageUrl;
        if (selectedImage) {
          imageUrl = await uploadFile(selectedImage, 'image');
          if (!imageUrl) return;
        }

        body = {
          name: formData.name,
          description: formData.description,
          imageUrl: imageUrl
        };
      } else if (dialogType === 'user') {
        url = selectedItem
          ? `http://localhost:5000/api/admin/users/${selectedItem.id}`
          : 'http://localhost:5000/api/admin/users';
        method = selectedItem ? 'PUT' : 'POST';
        body = {
          username: formData.username,
          email: formData.email,
          password: formData.password,
          role: formData.role
        };
      }

      const response = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(body)
      });

      if (!response.ok) {
        const errorText = await response.text();
        let errorMessage = 'Operation failed';
        try {
          const errorData = JSON.parse(errorText);
          errorMessage = errorData.message || errorMessage;
        } catch (e) {
          errorMessage = errorText || errorMessage;
        }
        throw new Error(errorMessage);
      }

      handleCloseDialog();
      fetchData();
      setSuccess(`${dialogType} ${selectedItem ? 'updated' : 'created'} successfully`);
    } catch (error) {
      setError(error.message);
    }
  };

  const handleDelete = async (type, id) => {
    if (window.confirm(`Are you sure you want to delete this ${type}?`)) {
      try {
        setError('');
        const url = type === 'game'
          ? `http://localhost:5000/api/game/${id}`
          : type === 'category'
          ? `http://localhost:5000/api/category/${id}`
          : `http://localhost:5000/api/admin/users/${id}`;

        const response = await fetch(url, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (!response.ok) {
          const errorText = await response.text();
          let errorMessage = `Failed to delete ${type}`;
          try {
            const errorData = JSON.parse(errorText);
            errorMessage = errorData.message || errorMessage;
          } catch (e) {
            errorMessage = errorText || errorMessage;
          }
          throw new Error(errorMessage);
        }

        fetchData();
        setSuccess(`${type} deleted successfully`);
      } catch (err) {
        setError('Failed to delete item: ' + err.message);
      }
    }
  };

  const handleRoleChange = async (userId, newRole) => {
    try {
      setError('');
      const response = await fetch(`http://localhost:5000/api/admin/users/${userId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({ role: newRole })
      });

      if (!response.ok) {
        const errorText = await response.text();
        let errorMessage = 'Failed to update role';
        try {
          const errorData = JSON.parse(errorText);
          errorMessage = errorData.message || errorMessage;
        } catch (e) {
          errorMessage = errorText || errorMessage;
        }
        throw new Error(errorMessage);
      }

      // Update local state
      setUsers(users.map(user => 
        user.id === userId ? { ...user, role: newRole } : user
      ));
      
      setSuccess('User role updated successfully');
    } catch (err) {
      setError('Failed to update user role: ' + err.message);
    }
  };

  const handleDeleteUser = async (userId) => {
    if (window.confirm('Are you sure you want to delete this user?')) {
      try {
        setError('');
        const response = await fetch(`http://localhost:5000/api/admin/users/${userId}`, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || 'Failed to delete user');
        }

        const data = await response.json();
        setSuccess(data.message || 'User deleted successfully');
        setUsers(users.filter(user => user.id !== userId));
      } catch (err) {
        setError('Failed to delete user: ' + err.message);
      }
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg">
      <Box sx={{ width: '100%', mt: 4 }}>
        <Tabs value={activeTab} onChange={handleTabChange}>
          <Tab label="Users" />
          <Tab label="Games" />
          <Tab label="Categories" />
        </Tabs>

        {error && (
          <Alert severity="error" sx={{ mt: 2 }}>
            {error}
          </Alert>
        )}

        {success && (
          <Alert severity="success" sx={{ mt: 2 }}>
            {success}
          </Alert>
        )}

        {activeTab === 0 && (
          <>
            <Box sx={{ mt: 2, mb: 2 }}>
              <Button
                variant="contained"
                color="primary"
                onClick={() => handleOpenDialog('user')}
              >
                Add User
              </Button>
            </Box>
            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Username</TableCell>
                    <TableCell>Email</TableCell>
                    <TableCell>Role</TableCell>
                    <TableCell>Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {users.map((user) => (
                    <TableRow key={user.id}>
                      <TableCell>{user.username}</TableCell>
                      <TableCell>{user.email}</TableCell>
                      <TableCell>
                        <FormControl>
                          <Select
                            value={user.role}
                            onChange={(e) => handleRoleChange(user.id, e.target.value)}
                          >
                            <MenuItem value="User">User</MenuItem>
                            <MenuItem value="Admin">Admin</MenuItem>
                          </Select>
                        </FormControl>
                      </TableCell>
                      <TableCell>
                        <Button
                          color="primary"
                          onClick={() => handleOpenDialog('user', user)}
                          sx={{ mr: 1 }}
                        >
                          Edit
                        </Button>
                        <Button
                          color="error"
                          onClick={() => handleDeleteUser(user.id)}
                        >
                          Delete
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </>
        )}

        {activeTab === 1 && (
          <>
            <Box sx={{ mt: 2, mb: 2 }}>
              <Button
                variant="contained"
                color="primary"
                onClick={() => handleOpenDialog('game')}
              >
                Add Game
              </Button>
            </Box>
            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Name</TableCell>
                    <TableCell>Description</TableCell>
                    <TableCell>Category</TableCell>
                    <TableCell>Image</TableCell>
                    <TableCell>Game URL</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell>Play Count</TableCell>
                    <TableCell>Last Played</TableCell>
                    <TableCell>Created At</TableCell>
                    <TableCell>Updated At</TableCell>
                    <TableCell>Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {games.map((game) => (
                    <TableRow key={game.id}>
                      <TableCell>{game.name}</TableCell>
                      <TableCell>{game.description}</TableCell>
                      <TableCell>
                        {categories.find(c => c.id === game.categoryId)?.name}
                      </TableCell>
                      <TableCell>
                        {game.imageUrl && (
                          <img 
                            src={game.imageUrl} 
                            alt={game.name} 
                            style={{ width: '50px', height: '50px', objectFit: 'cover' }} 
                          />
                        )}
                      </TableCell>
                      <TableCell>
                        <a href={game.gameUrl} target="_blank" rel="noopener noreferrer">
                          Play Game
                        </a>
                      </TableCell>
                      <TableCell>
                        {game.isActive ? 'Active' : 'Inactive'}
                      </TableCell>
                      <TableCell>{game.playCount}</TableCell>
                      <TableCell>
                        {game.lastPlayed ? new Date(game.lastPlayed).toLocaleDateString() : 'Never'}
                      </TableCell>
                      <TableCell>
                        {new Date(game.createdAt).toLocaleDateString()}
                      </TableCell>
                      <TableCell>
                        {game.updatedAt ? new Date(game.updatedAt).toLocaleDateString() : 'Never'}
                      </TableCell>
                      <TableCell>
                        <Button
                          color="primary"
                          onClick={() => handleOpenDialog('game', game)}
                          sx={{ mr: 1 }}
                        >
                          Edit
                        </Button>
                        <Button
                          color="error"
                          onClick={() => handleDelete('game', game.id)}
                        >
                          Delete
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </>
        )}

        {activeTab === 2 && (
          <>
            <Box sx={{ mt: 2, mb: 2 }}>
              <Button
                variant="contained"
                color="primary"
                onClick={() => handleOpenDialog('category')}
              >
                Add Category
              </Button>
            </Box>
            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Name</TableCell>
                    <TableCell>Description</TableCell>
                    <TableCell>Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {categories.map((category) => (
                    <TableRow key={category.id}>
                      <TableCell>{category.name}</TableCell>
                      <TableCell>{category.description}</TableCell>
                      <TableCell>
                        <Button
                          color="primary"
                          onClick={() => handleOpenDialog('category', category)}
                          sx={{ mr: 1 }}
                        >
                          Edit
                        </Button>
                        <Button
                          color="error"
                          onClick={() => handleDelete('category', category.id)}
                        >
                          Delete
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </>
        )}

        <Dialog open={openDialog} onClose={handleCloseDialog}>
          <DialogTitle>
            {selectedItem ? `Edit ${dialogType}` : `Add ${dialogType}`}
          </DialogTitle>
          <DialogContent>
            {dialogType === 'game' ? (
              <>
                <TextField
                  autoFocus
                  margin="dense"
                  label="Name"
                  fullWidth
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                />
                <TextField
                  margin="dense"
                  label="Description"
                  fullWidth
                  multiline
                  rows={4}
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                />
                <Box sx={{ mt: 2 }}>
                  <Typography variant="subtitle1">Image</Typography>
                  <FormControl component="fieldset" sx={{ mb: 2 }}>
                    <Box sx={{ display: 'flex', gap: 2 }}>
                      <FormControl>
                        <RadioGroup
                          row
                          value={imageUploadMethod}
                          onChange={(e) => setImageUploadMethod(e.target.value)}
                        >
                          <FormControlLabel value="url" control={<Radio />} label="URL" />
                          <FormControlLabel value="upload" control={<Radio />} label="Upload" />
                        </RadioGroup>
                      </FormControl>
                    </Box>
                  </FormControl>
                  {imageUploadMethod === 'url' ? (
                    <TextField
                      fullWidth
                      label="Image URL"
                      value={formData.imageUrl}
                      onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                    />
                  ) : (
                    <>
                      <input
                        type="file"
                        accept="image/*"
                        onChange={handleImageChange}
                        style={{ marginTop: '8px' }}
                      />
                      {formData.imageUrl && !selectedImage && (
                        <Typography variant="caption" display="block" sx={{ mt: 1 }}>
                          Current image: {formData.imageUrl}
                        </Typography>
                      )}
                    </>
                  )}
                </Box>
                <Box sx={{ mt: 2 }}>
                  <Typography variant="subtitle1">Game File</Typography>
                  <FormControl component="fieldset" sx={{ mb: 2 }}>
                    <Box sx={{ display: 'flex', gap: 2 }}>
                      <FormControl>
                        <RadioGroup
                          row
                          value={gameUploadMethod}
                          onChange={(e) => setGameUploadMethod(e.target.value)}
                        >
                          <FormControlLabel value="url" control={<Radio />} label="URL" />
                          <FormControlLabel value="upload" control={<Radio />} label="Upload" />
                        </RadioGroup>
                      </FormControl>
                    </Box>
                  </FormControl>
                  {gameUploadMethod === 'url' ? (
                    <TextField
                      fullWidth
                      label="Game URL"
                      value={formData.gameUrl}
                      onChange={(e) => setFormData({ ...formData, gameUrl: e.target.value })}
                    />
                  ) : (
                    <>
                      <input
                        type="file"
                        accept=".zip,.rar,.7z"
                        onChange={handleGameFileChange}
                        style={{ marginTop: '8px' }}
                      />
                      {formData.gameUrl && !selectedGameFile && (
                        <Typography variant="caption" display="block" sx={{ mt: 1 }}>
                          Current game file: {formData.gameUrl}
                        </Typography>
                      )}
                    </>
                  )}
                </Box>
                <FormControl fullWidth margin="dense">
                  <Select
                    value={formData.categoryId}
                    onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                    label="Category"
                  >
                    {categories.map((category) => (
                      <MenuItem key={category.id} value={category.id}>
                        {category.name}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
                <FormControlLabel
                  control={
                    <Switch
                      checked={formData.isActive}
                      onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                    />
                  }
                  label="Active"
                />
              </>
            ) : dialogType === 'category' ? (
              <>
                <TextField
                  autoFocus
                  margin="dense"
                  label="Name"
                  fullWidth
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                />
                <TextField
                  margin="dense"
                  label="Description"
                  fullWidth
                  multiline
                  rows={4}
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                />
                <Box sx={{ mt: 2 }}>
                  <Typography variant="subtitle1">Image</Typography>
                  <FormControl component="fieldset" sx={{ mb: 2 }}>
                    <Box sx={{ display: 'flex', gap: 2 }}>
                      <FormControl>
                        <RadioGroup
                          row
                          value={imageUploadMethod}
                          onChange={(e) => setImageUploadMethod(e.target.value)}
                        >
                          <FormControlLabel value="url" control={<Radio />} label="URL" />
                          <FormControlLabel value="upload" control={<Radio />} label="Upload" />
                        </RadioGroup>
                      </FormControl>
                    </Box>
                  </FormControl>
                  {imageUploadMethod === 'url' ? (
                    <TextField
                      fullWidth
                      label="Image URL"
                      value={formData.imageUrl}
                      onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                    />
                  ) : (
                    <>
                      <input
                        type="file"
                        accept="image/*"
                        onChange={handleImageChange}
                        style={{ marginTop: '8px' }}
                      />
                      {formData.imageUrl && !selectedImage && (
                        <Typography variant="caption" display="block" sx={{ mt: 1 }}>
                          Current image: {formData.imageUrl}
                        </Typography>
                      )}
                    </>
                  )}
                </Box>
              </>
            ) : (
              <>
                {!selectedItem && (
                  <TextField
                    autoFocus
                    margin="dense"
                    label="Username"
                    fullWidth
                    value={formData.username}
                    onChange={(e) => setFormData({ ...formData, username: e.target.value })}
                  />
                )}
                <TextField
                  margin="dense"
                  label="Email"
                  fullWidth
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                />
                <TextField
                  margin="dense"
                  label="Password"
                  type="password"
                  fullWidth
                  value={formData.password}
                  onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                />
                <FormControl fullWidth margin="dense">
                  <Select
                    value={formData.role}
                    onChange={(e) => setFormData({ ...formData, role: e.target.value })}
                  >
                    <MenuItem value="User">User</MenuItem>
                    <MenuItem value="Admin">Admin</MenuItem>
                  </Select>
                </FormControl>
              </>
            )}
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDialog}>Cancel</Button>
            <Button onClick={handleSubmit} color="primary">
              {selectedItem ? 'Update' : 'Create'}
            </Button>
          </DialogActions>
        </Dialog>
      </Box>
    </Container>
  );
};

export default AdminDashboard; 